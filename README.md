# CryptoTrading
Crypto trading is an application which consists of PositionsService and RatesService. Positions service listens from updated from the rates service api, recalculates profit/loss from open positions and loads initial positions from a csv file. On the other side, rates service fetches rates from CoinMarketCap API and detects significant price changes.

Authentication CMC:
https://coinmarketcap.com/api/documentation/v1/#section/Authentication

Preferred method via authorisation via header, Key-Value pair

Api to be used: https://coinmarketcap.com/api/documentation/v1/#operation/getV1CryptocurrencyListingsLatest
