from google.appengine.ext import db
from b58 import get_rand_addr
import datetime

class Wallet(db.Model):
    token = db.StringProperty(required=True)
    addr = db.StringProperty(required=True)
    balance = db.FloatProperty(required=True)
    
    @classmethod
    def gen_rand(cls):
        return cls(addr = get_rand_addr(), balance = 0.0, token=get_rand_addr("T"))

    @classmethod
    def get_by_token(cls, token):
        return cls.all().filter("token =", token).get()

class GameResult(db.Model):
    owner = db.ReferenceProperty(Wallet, required=True)
    timestamp = db.DateTimeProperty(auto_now_add=True, required=True)
    token = db.StringProperty(required=True)
    score = db.IntegerProperty()
    
    @classmethod
    def gen_new(cls, w):
        return cls(owner = w, score = 0, token=get_rand_addr("T"))

    @classmethod
    def get_top(cls, n, from_date = None):
        if not from_date:
            week = datetime.timedelta(weeks=1)
            from_date = datetime.datetime.now() - week
        recent = list(cls.all().filter("timestamp >", from_date).run())
        recent = sorted(recent,  key=lambda gr: gr.score, reverse=True)
        n = n if len(recent) > n else len(recent)
        return recent[:n]
    
    @classmethod
    def get_by_token(cls, token):
        return cls.all().filter("token =", token).get()



PRIZE_POOL_ADDR = "17jAJk4DxRDasSPm2p6Qfge9d4vxkms1cU"
class PrizePool(Wallet):
    @classmethod
    def get(cls):
        instance = cls.all().get()
        if not instance:
            instance = cls(balance = 0.0, addr = PRIZE_POOL_ADDR, token="prize-pool-"+PRIZE_POOL_ADDR)
            instance.put()
        return instance

HOUSE_ADDR = "1EGsLWgwdx9J78JbAjuGjwSVT8g4Br3uE5"
class HousePool(Wallet):
    @classmethod
    def get(cls):
        instance = cls.all().get()
        if not instance:
            instance = cls(balance = 0.0, addr = HOUSE_ADDR, token = "house-pool-"+HOUSE_ADDR)
            instance.put()
        return instance






    
    

