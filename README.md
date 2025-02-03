# CryptoTrading
Crypto trading is an application which consists of PositionsService and RatesService. Positions service listens from updated from the rates service api, recalculates profit/loss from open positions and loads initial positions from a csv file. On the other side, rates service fetches rates from CoinMarketCap API and detects significant price changes.

Authentication CMC:
https://coinmarketcap.com/api/documentation/v1/#section/Authentication

Preferred method via authorisation via header, Key-Value pair

Api to be used: https://coinmarketcap.com/api/documentation/v1/#operation/getV1CryptocurrencyListingsLatest

With docker, the following command was used to create a container for the rabbitmq.
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:management

Port 5672 is where rabbitmq will be hosted on. 
From the browser after running the next command can be shown the rabbitmq portal via http://localhost:15672/

Port 5672 is for visual studio and 15672 is for the browser portal.

Solution is Called CryptoTradingSystem. There are two projects inside: RatesService and PositionsService.

RatesService is a .Net api which communicates with CoinMarketCap api, gets the json file, deserializes the necessary information, loads or creates a new json file with the old rates, compares the rates, if greater than or equal to 5 percent then queues a message to rabbit mq to notify PositionsService.

PositionsService (Worker Application):

This is a background worker that listens to RabbitMQ for messages from RatesService.
Every 10 seconds, it checks for any rate change messages.
If the rate change is 5% or more, it calculates the profit and loss (P&L) based on the new rate and logs the result to the console.
The worker runs continuously, processing messages until it is stopped.

Key Components:
RabbitMQ: The message queue used for communication between the two services. When a significant rate change is detected by the RatesService, a message is sent to PositionsService to handle the necessary calculations.

Rate Change Logic: The RateChecker compares the latest rates with stored rates via method CheckPercentageChange and triggers the message queue only if a 5% or greater change is detected.
