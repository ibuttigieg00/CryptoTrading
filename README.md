# CryptoTrading
Crypto trading is an application which consists of PositionsService and RatesService. Positions service listens from updated from the rates service api, recalculates profit/loss from open positions and loads initial positions from a csv file. On the other side, rates service fetches rates from CoinMarketCap API and detects significant price changes.

Authentication CMC:
https://coinmarketcap.com/api/documentation/v1/#section/Authentication

Preferred method via authorisation via header, Key-Value pair

Api to be used: https://coinmarketcap.com/api/documentation/v1/#operation/getV1CryptocurrencyListingsLatest

With docker, the following commands where run to create a container for the rabbitmq.
Port 5672 is where rabbitmq will be hosted on. From the browser after running the next command
can be shown the rabbitmq portal via http://localhost:15672/

5672 is for visual studio and 15672 is for the browser portal

docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:management
