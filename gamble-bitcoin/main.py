#!/usr/bin/env python


import webapp2, json, jinja2, os
from model import Wallet
from blockchain import callback_secret_valid, get_tx, get_block, payment

jinja_environment = jinja2.Environment(
    loader=jinja2.FileSystemLoader(os.path.dirname(__file__)))


MINOR_ADDRESS = "1Fdz9kvAAurZYVE5ZchHo2iR2k8xc7BD7D"
MAJOR_ADDRESS = "15vaMvzo467iVGLUTEAq28B13QoNhkVa1B"
HEX_ADDRESS = "178tqyUVZchMWfWA8onCgJS18kmrFwKCLZ"

HOUSE_ADDRESS = "1FkYYQBStF5z9Hv3JQ5p2ruHRembeqSokE"

BET_ADDRESSES = [MINOR_ADDRESS, MAJOR_ADDRESS, HEX_ADDRESS]

FULL_ALPHABET = "0123456789abcdef"
HOUSE_EDGE = 1.85/100.0
ADDRESS_WINNERS = {
    MINOR_ADDRESS: '01234',
    MAJOR_ADDRESS: '56789',
    HEX_ADDRESS:   'abcdef'
}

def calculate_payout(address):
    return  len(FULL_ALPHABET)/float(len(ADDRESS_WINNERS[address])) *(1- HOUSE_EDGE)

ADDRESS_PAYOUT =  {
    MINOR_ADDRESS: calculate_payout(MINOR_ADDRESS),
    MAJOR_ADDRESS: calculate_payout(MAJOR_ADDRESS),
    HEX_ADDRESS:   calculate_payout(HEX_ADDRESS)
}

class MainHandler(webapp2.RequestHandler):
    def get(self):
        for k,v in ADDRESS_WINNERS.items():
          self.response.out.write("%s -> %s (x%s payout)<br />" % (k,v, ADDRESS_PAYOUT[k]))
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

    def process_bet(self, tx_hash):
        tx = get_tx(tx_hash)
        pay_addr = tx.get("inputs")[0].get("prev_out").get("addr")
        outs = tx.get("out") or []
        block = get_block(tx.get("block_height"))
        if not block:
            return "block not found"
        block_hash = block.get("hash") or ""
        lucky_digit = block_hash[-1]
        
        for out in outs:
            addr = out.get("addr") or ""
            bet_value = out.get("value") or 1
            if addr in BET_ADDRESSES:
                if lucky_digit in ADDRESS_WINNERS[addr]:
                    payment_value = bet_value*ADDRESS_PAYOUT[addr]
                    result = payment(pay_addr, payment_value, HOUSE_ADDRESS)
                    if not result:
                        return "payment failed"
        return True
                
    
    def handle(self):
        secret = self.request.get("secret")
        if not callback_secret_valid(secret):
            return "error: secret"
        test = self.request.get("test") == "true"
        try:
            confirmations = int(self.request.get("confirmations"))
            tx = self.request.get("transaction_hash")
        except ValueError, e:
            return "error: value error"
        
        if not tx:
            return "error: no transaction_hash"
        
        if confirmations < 1:
            return "error: unconfirmed"
        
        if not test:
            result = self.process_bet(tx)
            if result is not True:
                return "error: process: "+result
        return "*ok*"

app = webapp2.WSGIApplication([
    ('/', MainHandler),
    # API
    ('/api/bootstrap', BootstrapHandler),
    ('/api/callback', CallbackHandler),
], debug=True)

