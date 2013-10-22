from google.appengine.ext import db
import datetime

PENDING, WIN, LOSS = 0, 1, 2
RESULTS = [PENDING, WIN, LOSS]
DATE_FORMAT = "%Y-%m-%d %H:%M:%S"

class Bet(db.Model):
    # this two rows uniquely identify the Bet (address, tx)
    betting_addr = db.StringProperty(required=True)
    bet_tx = db.StringProperty(required=True)
    
    timestamp = db.DateTimeProperty(required=True, auto_now_add=True)
    better = db.StringProperty(required=True)
    amount = db.IntegerProperty(required=True)
    result = db.IntegerProperty(required=True, choices=RESULTS)
    bet_block = db.StringProperty()
    pay_tx = db.StringProperty()
    
    
    def to_dict(self):
        d = db.to_dict(self)
        d["timestamp_str"] = d["timestamp"].strftime(DATE_FORMAT)
        return d

    @classmethod 
    def new(cls, better, betting_addr, bet_tx, amount):
        return cls(better=better, betting_addr=betting_addr, bet_tx=bet_tx, amount=amount, result=PENDING)

    @classmethod
    def get(cls, timestamp):
        return cls.all().filter("timestamp = ", timestamp).get()

    @classmethod
    def get_latest(cls):
        return cls.all().order('-timestamp').run(limit=10)
    
    

