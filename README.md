# TrainMonitor - Asp.Net Core 9 MVC application

## Demo:

Demo video link: https://res.cloudinary.com/dzuieskuw/video/upload/v1760431876/msedge_qoP46UMdmy_yjdf9h.mp4

![demo 1](https://res.cloudinary.com/dzuieskuw/image/upload/v1760429730/trains_pwof6r.png)

![demo 2](https://res.cloudinary.com/dzuieskuw/image/upload/v1760429730/trains2_eceexb.png)

![demo 3](https://res.cloudinary.com/dzuieskuw/image/upload/v1760429730/train3_ecnml9.png)

![demo 4](https://res.cloudinary.com/dzuieskuw/image/upload/v1760429922/train4_mpq0ik.png)



## Description:
TrainMonitor is an Asp.Net Core 9 MVC application that monitors train locations in real-time.
It uses SignalR to provide real-time updates to the client and server. 

The trains are continuously sent in random batches to the client at irregular intervals,
simulating real-time train movements. Each train appears for a randomly assigned lifetime (in ms) before
being removed and then re-added to the pool after a short pause period (in ms).  

The system ensures:
 - No duplicate trains are visible simultaneously.
 - Each train receives a new randomized display duration (in ms) on every reappearance.
 - Each train is removed after a random duration (in ms) assigned to it.

The live updates run indefinitely until the client disconnects or the connection is aborted.

Signalr logis is located in: **Hubs/TrainHub.cs class**

## Features:

- Asp.Net Core 9
- Dependency Injection
- Repository pattern for data access abstraction
- Service layer for business logic
- Unit of Work pattern for managing transactions
- Docker and docker-compose for containerization
- Entity Framework Core for data access
- Migrations to manage database schema changes
- Tables configurations via IEntityTypeConfiguration
- Fluent API for model configuration
- Razor Pages for UI
- MariaDB as the database
- Loading data via a JSON file
- Bootstrap 5 for styling
- FontAwesome for icons
- CSS for styling
- Responsive design
- JavaScript for interactivity
- jQuery for DOM manipulation and Form validation
- Ajax for asynchronous requests
- jQuery Unobtrusive Validation for client-side validation
- JSON Deserialization with System.Text.Json
- ViewModel for passing data to views
- DTOs for data transfer between layers
- SignalR for real-time web functionality
- SignalR notifications for real-time updates from the client and server