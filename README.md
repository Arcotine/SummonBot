# Summon Bot

Telegram chat bot that posts gifs using the Giphy API! Can also mirror some gifs to a Discord channel using a Webhook if it is entered while building.

## Building
Use docker to build as such:
`docker build -t <name of image> -f Docker/Dockerfile .`

## Running the bot
The bot must be run on a local machine. First, [build](#Building) the docker image above. To run the docker image, use the command:

`docker run --name <name of image> -e "DiscordURL=<Discord API Key>" -e "TelegramBotKey=<Telegram API Key>" -e "ImageLink=<Link to Summon Gif Image>" -e "GiphyKey=<Giphy API Key>"`