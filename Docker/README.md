# Docker Build

For all docker commands, navigate to the root project subdirectory (`SummonBot/`)
`cd ../`

To build, enter the command
`docker build -t <name of image> -f Docker/Dockerfile .`

To run the previously built docker image, use
`docker run --name <name of image> -e "DiscordURL=<Discord API Key>" -e "TelegramBotKey=<Telegram API Key>" -e "ImageLink=<Link to Gif Image>" -e "GiphyKey=<Giphy API Key>"`
