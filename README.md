# JWT login with refresh token

Example of a .NET Core Web API using MongoDB with JWT authentication and support for refresh tokens.

## Assumes these unique indexes are created

```javascript
db.getCollection('accounts').createIndex(
    { username: 1 },
    { unique: true, background: true }
);
db.getCollection('accounts').createIndex(
    { email_address: 1 },
    { unique: true, background: true }
);
db.getCollection('refresh_tokens').createIndex(
    { account_id: 1, value: 1 },
    { unique: true, background: true }
);
```

## Sample requests

### Create a new account

```http
POST https://localhost:5001/api/accounts
Content-Type: application/json

{
    "username": "alice",
    "password": "t0ps3cret",
    "surname": "Albright",
    "given_name": "Alice",
    "email_address": "alice.albright@example.com"
}
```

### Login

```http
POST https://localhost:5001/api/login
Content-Type: application/json

{
    "username": "alice",
    "password": "t0ps3cret"
}
```

### Refresh access token

```http
POST https://localhost:5001/api/login/refresh
Content-Type: application/json

{
    "access_token": "<ACCESS_TOKEN_JWT>",
    "refresh_token": "<REFRESH_TOKEN_RANDOM>"
}
```

### Get account details

```http
GET https://localhost:5001/api/accounts
Authorization: Bearer <ACCESS_TOKEN_JWT>
```
