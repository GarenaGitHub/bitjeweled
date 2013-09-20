from google.appengine.ext import db

class Wallet(db.Model):
    token = db.StringProperty(required=True)
    addr = db.StringProperty(required=True)
    balance = db.FloatProperty(required=True)
    




    
    

