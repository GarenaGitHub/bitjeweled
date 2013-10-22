#!/usr/bin/env python


import webapp2, json, jinja2, os, logging, datetime, calendar
from model import Bet, PENDING, LOSS, WIN
from blockchain import callback_secret_valid, get_tx, get_block, payment
from config import ADDRESS_PAYOUT, ADDRESS_WINNERS, calculate_odds,\
    BET_ADDRESSES, HOUSE_ADDRESS

JINJA_ENVIRONMENT = jinja2.Environment(
    loader=jinja2.FileSystemLoader(os.path.dirname(__file__)),
    extensions=['jinja2.ext.autoescape'])




class StaticHandler(webapp2.RequestHandler):
    def get(self, _):
        name = self.request.path.split("/")[1]
        if name == "":
            name = "index"
            
        values = {
            "name": name
        }
        
        try:
            self.response.write(JINJA_ENVIRONMENT.get_template("/templates/" + name + '.html').render(values))
        except IOError, e:
            self.error(404)
            self.response.write("404: %s not found! %s" % (name, e))

class JsonAPIHandler(webapp2.RequestHandler):
    def post(self):
        self.get()
    def get(self):
        resp = self.handle()
        self.response.headers['Content-Type'] = "application/json"
        dthandler = lambda obj: calendar.timegm(obj.utctimetuple())+obj.microsecond / 1000000.0 if isinstance(obj, datetime.datetime) else None
        self.response.write(json.dumps(resp, default=dthandler))

class BootstrapHandler(JsonAPIHandler):
    def handle(self):
        Bet.new("new", "15vaMvzo467iVGLUTEAq28B13QoNhkVa1B", "0abfc1746923", 1000).put()
        return {"success":True}
    
class BettingAddressesHandler(JsonAPIHandler):
    def handle(self):
        return [{"addr":addr,
                 "payout": ADDRESS_PAYOUT[addr],
                 "winners": ADDRESS_WINNERS[addr],
                 "odds": 1 / calculate_odds(addr)
                 } for addr in ADDRESS_PAYOUT]

class BetListHandler(JsonAPIHandler):
    def handle(self):
        return {"success": True, "list": [bet.to_dict() for bet in Bet.get_latest()]}

class BetInfoHandler(JsonAPIHandler):
    def handle(self):
        timestamp = self.request.get("timestamp")
        if not timestamp:
            return {"success": False, "error": "missing parameters"}
        timestamp = datetime.datetime.fromtimestamp(float(timestamp))
        bet = Bet.get(timestamp)
        if not bet:
            return {"success": False, "error": "bet not found"}
        
        return {"success": True, "bet": bet.to_dict()}

class CallbackHandler(webapp2.RequestHandler):
    tx = None
    block_hash = None
    bet_result = None
    pay_tx = None
    
    def get(self):
        self.response.out.write(self.handle())

    def get_tx(self, tx_hash):
        if not self.tx:
            self.tx = get_tx(tx_hash)
        return self.tx

    def get_pay_addr(self, tx):
        return tx.get("inputs")[0].get("prev_out").get("addr")
    
    def process_bet(self, tx, receiving_address):
        pay_addr = self.get_pay_addr(tx)
        outs = tx.get("out")
        if not outs:
            return "unable to retrieve tx outs."
            
        block = get_block(tx.get("block_height"))
        if not block:
            return "block not found"
        block_hash = block.get("hash") or "-"
        lucky_digit = block_hash[-1]
        
        for out in outs:
            addr = out.get("addr") or ""
            if addr != receiving_address:
                continue
            self.block_hash = block_hash
            bet_value = out.get("value") or 1
            if addr in BET_ADDRESSES:
                if lucky_digit in ADDRESS_WINNERS[addr]:
                    self.bet_result = WIN
                    payment_value = bet_value * ADDRESS_PAYOUT[addr]
                    result = payment(pay_addr, payment_value, HOUSE_ADDRESS)
                    if not result:
                        return "payment failed"
                    else:
                        self.pay_tx = result
                else:
                    self.bet_result = LOSS
                    
        return True
                
    
    def handle(self):
        secret = self.request.get("secret")
        if not callback_secret_valid(secret):
            return "error: secret"
        test = self.request.get("test") == "true"
        try:
            confirmations = int(self.request.get("confirmations"))
            tx_hash = self.request.get("transaction_hash")
            address = self.request.get("address")
            value = int(self.request.get("value"))
        except ValueError, e:
            return "error: value error"
        
        if not tx_hash:
            return "error: no transaction_hash"
    
        if not address:
            return "error: no address"
        
        if value <= 0:  # outgoing payment
            return "*ok*"
        
        if address not in BET_ADDRESSES:  # not a gamble transaction
            return "*ok*"
        
        if confirmations == 0:
            tx = self.get_tx(tx_hash)
            better = self.get_pay_addr(tx)
            if not tx:
                return "error: unable to retrieve tx."
            if not Bet.exists(address, tx_hash):
                bet = Bet.new(better=better, betting_addr=address, bet_tx=tx_hash, amount=value)
                bet.put()
        
        if confirmations < 1:
            return "error: unconfirmed."
        
        if not test:
            tx = self.get_tx(tx_hash)
            if not tx:
                return "error: unable to retrieve tx."
            result = self.process_bet(tx, address)
            
            if result is not True:
                return "error: process: " + result
            bet = Bet.get_by_data(address, tx_hash)
            if not bet:
                better = self.get_pay_addr(tx)
                bet = Bet.new(better, address, tx_hash, value)
            bet.result = self.bet_result
            bet.bet_block = self.block_hash
            bet.pay_tx = self.pay_tx
            bet.put()
        
        return "*ok*"

app = webapp2.WSGIApplication([
    ('/((?!api).)*', StaticHandler),
    # API
    
    # frontend
    ('/api/betting_addresses', BettingAddressesHandler),
    ('/api/bets/list', BetListHandler),
    ('/api/bets/get', BetInfoHandler),
    
    # backend
    ('/api/bootstrap', BootstrapHandler),
    ('/api/callback', CallbackHandler),
], debug=True)

