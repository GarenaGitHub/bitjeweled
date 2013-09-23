#!/usr/bin/env python


import webapp2, json, jinja2, os
from model import Wallet
from blockchain import callback_secret_valid, get_tx, get_block, payment

jinja_environment = jinja2.Environment(
    loader=jinja2.FileSystemLoader(os.path.dirname(__file__)))

{"addr_tag":"SatoshiDICE 18%","n":0,"value":20000000,"addr":"1dice7EYzJag7SxkdKXLr8Jn14WUb3Cf1","tx_index":90827863,"type":0,"addr_tag_link":"http:\/\/satoshidice.com"}

MINOR_ADDRESS = "1Fdz9kvAAurZYVE5ZchHo2iR2k8xc7BD7D"
MAJOR_ADDRESS = "15vaMvzo467iVGLUTEAq28B13QoNhkVa1B"
HEX_ADDRESS = "178tqyUVZchMWfWA8onCgJS18kmrFwKCLZ"

HOUSE_ADDRESS = "1FkYYQBStF5z9Hv3JQ5p2ruHRembeqSokE"

BET_ADDRESSES = [MINOR_ADDRESS, MAJOR_ADDRESS, HEX_ADDRESS]

ADDRESS_WINNERS = {
    MINOR_ADDRESS: '01234',
    MAJOR_ADDRESS: '56789',
    HEX_ADDRESS:   'abcdef'
}

ADDRESS_PAYOUT =  {
    MINOR_ADDRESS: 3.0,
    MAJOR_ADDRESS: 3.0,
    HEX_ADDRESS:   3.0
}

class MainHandler(webapp2.RequestHandler):
    def get(self):
        self.response.out.write("It works!")
        # template = jinja_environment.get_template('index.html')
        # self.response.out.write(template.render({}))

class JsonAPIHandler(webapp2.RequestHandler):
    def post(self):
        self.get()
    def get(self):
        resp = self.handle()
        self.response.write(json.dumps(resp))

class BootstrapHandler(JsonAPIHandler):
    def handle(self):
        return {"success":True}


class CallbackHandler(JsonAPIHandler):
    
    def do_pay(self, pay_addr, payment_value):
        return payment(pay_addr, payment_value, HOUSE_ADDRESS)
        
    def process_bet(self, tx_hash):
        tx = get_tx(tx_hash)
        pay_addr = tx.get("prev_out")[0].get("addr")
        outs = tx.get("out") or []
        block_height = tx.get("block_height")
        block = get_block(block_height)
        if not block:
            return None
        block_hash = block.get("hash") or ""
        lucky_digit = block_hash[-1]
        
        for out in outs:
            addr = out.get("addr") or ""
            bet_value = out.get("value") or 1
            if addr in BET_ADDRESSES:
                win = lucky_digit in ADDRESS_WINNERS[addr]
                if win:
                    payment_value = bet_value*ADDRESS_PAYOUT(addr)
                    result = self.do_pay(pay_addr, payment_value)
                    if not result:
                        return None
                
    
    def handle(self):
        secret = self.request.get("secret")
        if not callback_secret_valid(secret):
            return "error: secret"
        test = self.request.get("test") == "true"
        try:
            value = long(self.request.get("value"))
            confirmations = int(self.request.get("confirmations"))
            tx = self.request.get("transaction_hash")
        except ValueError, e:
            return "error: value error"
        
        if value < 0 or not tx:
            return "*ok*"
        
        if confirmations < 1:
            return "error: unconfirmed"
        
        if not test:
            result = self.process_bet(tx)
            if not result:
                return "error: process"
        return "*ok*"

app = webapp2.WSGIApplication([
    ('/', MainHandler),
    # API
    ('/api/bootstrap', BootstrapHandler),
    ('/api/callback', CallbackHandler),
], debug=True)

