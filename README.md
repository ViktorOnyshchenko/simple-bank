Banking solution is simple app that covers such requirements:
1.1 Create account using user information (firstname, lastname, phone number - unique).
1.2 Get account details by account number (auto increment field that is started from 10000). There is a check for the existence of an account.
1.3 Get all account with short information. There is a check for any existence of accounts
2.1 Deposit funds into account using account number and amount of funds. There is a check for the existence of an account.
2.2 Withdraw funds from account using account number and amount of funds. There is a check for enough money.
2.3 Transfer funds between accounts using sender account number, receiver account number and amount of funds.There are 2 checks: for the existence of an account and for enough money.

Architecture based on such as pattern: N-layer (separate api logic and datalayer, but there is neglect of the business layer), unitofwork (transaction for transfer funds), repository (easy handle operation with database).
As datastorage is in-memory database using ef core 9. Entities relations were configured using Fluent API.
There are unit tests that are covered reposotories logic.
Locally this project be run using swagger endpoing and IIS Express.
