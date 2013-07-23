#!/usr/bin/env python


import webapp2, json, jinja2, os
from model import Wallet, GameResult, PrizePool, HousePool

jinja_environment = jinja2.Environment(
    loader=jinja2.FileSystemLoader(os.path.dirname(__file__)))



PLAY_COST = 0.25
DISTRIBUTION_PERCENTAGES = [0.5,0.3,0.15]
  

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

class CreateHandler(JsonAPIHandler):
    def handle(self):
        w = Wallet.gen_rand()
        w.put()
        return {"success": True, "address": w.addr, "token" : w.token}

class WalletActionHandler(JsonAPIHandler):
    def handle(self):
        token = self.request.get("t")
        w = Wallet.get_by_token(token)
        if not w:
            return {"success": False, "reason": "invalid address token"}
        return self.handle_wallet(w)
    
class DepositHandler(WalletActionHandler):
    def handle_wallet(self, w):
        deposit = self.request.get("d")
        try:
            w.balance += float(deposit)
            w.put()
            return {"success": True, "balance": w.balance}
        except ValueError:
            return {"success": False, "reason": "invalid deposit"}

class PlayHandler(WalletActionHandler):
    def handle_wallet(self, w):
        gr = GameResult.gen_new(w)
        w.balance -= PLAY_COST
        pool = PrizePool.get()
        pool.balance += PLAY_COST
        gr.put()
        pool.put()
        w.put()
        return {"success": True, "token": str(gr.token)}
        
class ScoreSubmitHandler(JsonAPIHandler):
    def handle(self):
        token = self.request.get("t")
        score = int(self.request.get("s"))
        
        gr = GameResult.get_by_token(token)
        if not gr:
            return {"success": False}
        if gr.score != 0:
            return {"success": False}
        gr.score = score
        gr.put()
        return {"success": True}
    

class BalanceHandler(WalletActionHandler):
    def handle_wallet(self, w):
        return {"success": True, "balance": w.balance}


class TopHandler(JsonAPIHandler):
    def handle(self):
        n = int(self.request.get("n"))
        top = GameResult.get_top(n)
        return {"success": True, "top": [{
                    "addr" : gr.owner.addr,
                    "score" : gr.score
                  }
                 for gr in top]}

class DistributeHandler(JsonAPIHandler):
    def handle(self):
        assert sum(DISTRIBUTION_PERCENTAGES) < 1
        n = len(DISTRIBUTION_PERCENTAGES)
        pool = PrizePool.get()
        house = HousePool.get()
        top = GameResult.get_top(n)
        
        prize = pool.balance
        
        txs = []
        for i,gr in enumerate(top):
            share = prize * DISTRIBUTION_PERCENTAGES[i]
            gr.owner.balance += share
            pool.balance -= share
            gr.owner.put()
            txs.append((gr.owner.addr, share))
        rest = pool.balance
        house.balance += rest
        pool.balance = 0.0
        pool.put()
        house.put()
        txs.append((house.addr, rest))
        
        return {"success": True, "txs": txs}
            


app = webapp2.WSGIApplication([
    ('/', MainHandler),
    ('/api/create', CreateHandler),
    ('/api/deposit', DepositHandler),
    ('/api/play', PlayHandler),
    ('/api/score', ScoreSubmitHandler),
    ('/api/balance', BalanceHandler),
    ('/api/top', TopHandler),
    ('/api/distribute', DistributeHandler),
], debug=True)

