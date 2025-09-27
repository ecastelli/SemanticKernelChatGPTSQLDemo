# Semantic Kernel Chat GPT SQL Demo
Simple demonstration of using a .NET 8 API and Microsoft Semantic Kernel with Chat GPT to have Chat GPT generate and return SQL queries for execution against a local database.  Leverages the sales data in the Adventure Works database using SQL Server Express Local DB. 

In addition, the API demonstrates how to persist chat sessions using a separate database.  This in turn allows Chat GPT to respond to follow up questions by pushing the chat history back to Chat GPT on follow up questions.

# Prerequisites
-Sign up for a Chat GPT API account (https://platform.openai.com/api-keys) and update ChatGptApiKey setting in the appsettings.json in the API project with your api key.

-Install SQL Server Express Local DB if not already installed (https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb?view=sql-server-ver17)

-API should automatically attach the MDFs for Adventure Works and Chat Sessions, but you may have to manually attach these if this fails (in API/App_Data folder).

<img width="2319" height="674" alt="Screenshot 2025-09-27 111633" src="https://github.com/user-attachments/assets/7aea5a77-940b-4d4d-9821-8e49bf3a00d3" />
