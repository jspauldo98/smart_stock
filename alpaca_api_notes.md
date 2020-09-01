# What can be done ussing Alpaca API...
##### https://alpaca.markets/docs/api-documentation/

## Market Data:

### Exchanges
-IEX
- NYSE National
- Nasdaq BX
- Nasdeq PSX
- NYSE Chicago

### Authentication
Every private API call requires key-based authentication. API keys can be acquired in the developer web console. The client must provide a pair of API key ID and secret key in the HTTP request headers named APCA-API-KEY-ID and APCA-API-SECRET-KEY respectively.

Here is an example using curl showing how to authenticate with the API.

```
curl -X GET \
    -H "APCA-API-KEY-ID: {YOUR_API_KEY_ID}" \
    -H "APCA-API-SECRET-KEY: {YOUR_API_SECRET_KEY}"\
    https://{apiserver_domain}/v2/account
```

### Trade Scheme
- ev -> event name always "T"
- T -> symbol
- i -> trade ID
- x -> exchange code where the trade occured
- p -> trade price
- s -> trade size
- t -> epoch timestamp in nanoseconds
- c -> conditional flags
- z -> tape ID

```
{
  "ev": "T",
  "T": "SPY",
  "i": 117537207,
  "x": 2,
  "p": 283.63,
  "s": 2,
  "t": 1587407015152775000,
  "c": [
    14,
    37,
    41
  ],
  "z": 2
}
```

### Quote Schema
- ev -> even name, always "Q"
- T -> symbol
- x -> exchange code for bid quote
- p -> bid price
- s -> bid size
- X -> exchange code for ask price
- P -> ask price
- S -> ask price
- c -> condition flags
- t -> epoch timestamp in nanseconds

```
{
  "ev": "Q",
  "T": "SPY",
  "x": 17,
  "p": 283.35,
  "s": 1,
  "X": 17,
  "P": 283.4,
  "S": 1,
  "c": [
    1
  ],
  "t": 1587407015152775000
}
``` 
