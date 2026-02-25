# Badminton Courts & Players API

API for creating courts and managing players (set checkboxes, payment). All endpoints include null checks and validation. All user-facing messages are in English.

## Base URL
- Development: `https://localhost:7291` or `http://localhost:8080` (depends on config)
- API prefix: `api/courts`, `api/courts/{courtId}/players`

## Response format
- Success: `{ "status": true, "message": "...", "data": ... }`
- Error: `{ "status": false, "errorMessage": "..." }`

---

## Courts

### POST `api/courts` — Create court
**Body (JSON):**
```json
{
  "name": "Court A - Floor 1",
  "password": "1234"
}
```
- **Validation:** `name` required, min 3 characters; `password` required, min 4 characters.
- **Messages:** e.g. "Court created successfully", "Court name is required", "Password must be at least 4 characters".
- **Response data:** Court with `id`, `name`, `password`, `createdDate` (returned only on success).

### GET `api/courts` — List courts
- **Response data:** Array of `CourtResponse` (no `password`): `id`, `name`, `createdDate`.

### GET `api/courts/{id}` — Get court by ID
- **Validation:** `id` must not be null/empty.
- **Messages:** "Invalid court ID", "Court not found".
- **Response data:** `CourtResponse` (no `password`).

### POST `api/courts/{id}/verify-password` — Verify court password
**Body (JSON):**
```json
{
  "password": "1234"
}
```
- **Messages:** "Password verified successfully", "Invalid password", "Password is required".
- **Response data (success):** `{ "verified": true }`.

---

## Players

### GET `api/courts/{courtId}/players` — List players by court
- **Response data:** Array of `PlayerResponse`: `id`, `courtId`, `name`, `checkboxes` (bool[12]), `isPaid`, `createdDate`.

### POST `api/courts/{courtId}/players` — Add player
**Body (JSON):**
```json
{
  "courtId": "<courtId from URL or body>",
  "name": "Player Name"
}
```
- **Validation:** `name` required, min 2 characters; court must exist.
- **Messages:** "Player added successfully", "Player name is required", "Court not found".
- **Response data:** `PlayerResponse` of created player (checkboxes = 12 false).

### PATCH `api/courts/{courtId}/players/checkbox` — Update one set checkbox (realtime)
**Body (JSON):**
```json
{
  "playerId": "...",
  "checkboxIndex": 0,
  "isChecked": true
}
```
- **Validation:** `playerId` required; `checkboxIndex` 0–11.
- **Messages:** "Set updated successfully", "Player not found", "Player ID is required".

### PATCH `api/courts/{courtId}/players/payment` — Update payment status
**Body (JSON):**
```json
{
  "playerId": "...",
  "isPaid": true
}
```
- **Validation:** `playerId` required.
- **Messages:** "Payment status updated successfully", "Player not found".

---

## Notes
- MongoDB collections: `Courts`, `Players` (database: `Todolist_Paging`).
- Court password is **not** returned in GET list or GET by id; only on create (CreateCourt).
- Realtime: FE uses SignalR; REST API is ready; add SignalR hub later to broadcast if needed.
