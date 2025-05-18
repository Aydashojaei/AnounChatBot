# AnounChatBot

**AnounChatBot** is a Telegram bot that enables anonymous chatting between users without revealing their identities. The bot implements a coin system to limit the number of chats each user can have.

## Features

- Anonymous one-on-one chat with random user matching
- Coin-based system to limit chat sessions (users start with 5 coins)
- Ability to purchase coins to continue chatting after running out
- Waiting queue management and automatic user pairing
- Option to leave the chat and disconnect
- User data storage and management in a database

## How to Use

1. Add the bot to your Telegram.
2. Send the `/start` command to register.
3. Click the "🗨️ Anonymous Chat" button to start chatting.
4. When your coins run out, you must purchase more to continue chatting.

## Technologies Used

- C# programming language
- ASP.NET Core
- Entity Framework Core
- SQL Server database
- Telegram.Bot API

## Important Notes

- Sensitive information such as bot tokens and database connection strings are stored in the `appsettings.json` file, which should be excluded from GitHub.
- For feature requests or bug fixes, please submit a Pull Request.
