#!/usr/bin/env python


import webapp2, json, jinja2, os
from model import Wallet
from blockchain import callback_secret_valid

jinja_environment = jinja2.Environment(
    loader=jinja2.FileSystemLoader(os.path.dirname(__file__)))



  

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
    def handle(self):
        secret = self.request.get("secret")
        if not callback_secret_valid(secret):
            return "error: secret"
        test = self.request.get("test") == "true"
        
        return "*ok*"

app = webapp2.WSGIApplication([
    ('/', MainHandler),
    # API
    ('/api/bootstrap', BootstrapHandler),
    ('/api/callback', CallbackHandler),
], debug=True)

