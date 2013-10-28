
MINOR_ADDRESS = "1Fdz9kvAAurZYVE5ZchHo2iR2k8xc7BD7D"
MAJOR_ADDRESS = "15vaMvzo467iVGLUTEAq28B13QoNhkVa1B"
HEX_ADDRESS = "178tqyUVZchMWfWA8onCgJS18kmrFwKCLZ"

HOUSE_ADDRESS = "1FkYYQBStF5z9Hv3JQ5p2ruHRembeqSokE"

BET_ADDRESSES = [MINOR_ADDRESS, MAJOR_ADDRESS, HEX_ADDRESS]

FULL_ALPHABET = "0123456789abcdef"
HOUSE_EDGE = 1.85 / 100.0
ADDRESS_WINNERS = {
    MINOR_ADDRESS: '01234',
    MAJOR_ADDRESS: '56789',
    HEX_ADDRESS:   'abcdef'
}

ADDRESS_TYPE = {
    MINOR_ADDRESS: 'Low',
    MAJOR_ADDRESS: 'High',
    HEX_ADDRESS:   'Hex'            
}

def calculate_odds(address):
    return len(FULL_ALPHABET) / float(len(ADDRESS_WINNERS[address]))
def calculate_payout(address):
    return calculate_odds(address) * (1 - HOUSE_EDGE)

ADDRESS_PAYOUT = {
    MINOR_ADDRESS: calculate_payout(MINOR_ADDRESS),
    MAJOR_ADDRESS: calculate_payout(MAJOR_ADDRESS),
    HEX_ADDRESS:   calculate_payout(HEX_ADDRESS)
}
