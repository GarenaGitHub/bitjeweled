 
alphabet = '123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz'
base_count = len(alphabet)

from random import choice

def get_rand_addr(version = 0):
    return str(version)+("".join([choice(alphabet) for _ in xrange(33)]))

def encode(num):
    """ Returns num in a base58-encoded string """
    encode = ''
   
    if (num < 0):
        return ''
   
    while (num >= base_count):    
        mod = num % base_count
        encode = alphabet[mod] + encode
        num = num // base_count
 
    if (num):
        encode = alphabet[num] + encode
 
    return encode
 
def decode(s):
    """ Decodes the base58-encoded string s into an integer """
    decoded = 0
    multi = 1
    s = s[::-1]
    for char in s:
        decoded += multi * alphabet.index(char)
        multi = multi * base_count
       
    return decoded
