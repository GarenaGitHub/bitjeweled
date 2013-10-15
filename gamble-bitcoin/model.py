from google.appengine.ext import db


PENDING, WIN, LOSS = 0, 1, 2
RESULTS = [PENDING, WIN, LOSS]

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
    

    @classmethod 
    def new(cls, better, betting_addr, bet_tx, amount):
        return cls(better=better, betting_addr=betting_addr, bet_tx=bet_tx, amount=amount, result=PENDING)

    @classmethod
    def get(cls, bet_tx, betting_addr):
        return cls.all().filter("bet_tx = ", bet_tx).filter("betting_addr = ", betting_addr).get()

    
    

