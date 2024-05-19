
# Link Shortener API

This repository contains the base code for a potential Link Shorter API. Although not yet deployed as a service, the code provides a robust and efficient solution for transforming long, complicated URLs into shorter, more manageable addresses. This project represents an opportunity to explore and learn about link optimization techniques.


## Installation

1. Clone this repository on your local machine with `git clone https://github.com/your-user/your-repository.git`.
2. Navigate to the project directory with `cd route/to/your/project`.
3. Install the required dependencies with `dotnet restore`.
4. Run migrations to create the database with:
     ```bash
     dotnet ef migrations add InitialCreate
     dotnet ef database update
    
## Deployment

Run the project with:

```bash
  dotnet run
```


## API Reference

#### Short Url

```http
  POST /api/shorten
```
```json
{
    "Url": "https://www.ejemplo.com"
}
```

The API will return you a short URL that you can use instead of the original URL.
## Contributing

Contributions are always welcome!

See `contributing.md` for ways to get started.

Please adhere to this project's `code of conduct`.


## License

[GNU GPLv3](https://choosealicense.com/licenses/gpl-3.0/)
