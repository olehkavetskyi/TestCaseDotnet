# .NET API Service for Transaction Management - TestCaseDotnet

This app imports a transactions list from an Excel file provided by the user. The user can change the transaction status and export the filtered list as a CSV file.

## Tests

This app uses xUnit framework for unit testing.

![photo_2023-07-31_19-05-10](https://github.com/olehkavetskyi/TestCaseDotnet/assets/110283090/5d4582c0-3d01-488b-bf2a-816b21724321)

## Configuration

Add this snippet of code to `appsettings.json` file:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your connection string"
  },
  "Token": {
    "Key": "your key",
    "Issuer": "your issuer",
    "Audience": "your audience"
  }
}

