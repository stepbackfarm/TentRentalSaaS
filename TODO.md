# TODO

- [ ] Refactor the API to use Data Transfer Objects (DTOs) instead of returning Entity Framework entities directly. This will provide a more robust solution to the JSON serialization circular reference issue and improve the API's architecture.
    - Currently, a quick fix has been applied in `Program.cs` to ignore circular references during serialization.
    - The `BookingService` and `BookingsController` should be updated to use a `BookingResponseDto`.
