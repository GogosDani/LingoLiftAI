# LingoLiftAI

## About The Project

LingoLiftAi is an application for language learning. Users can create flashcards by themselves or with the help of AI. They can also join daily challenges and play against other players.

### Key Features:
- **Language Test After Registration**
- **Create Own Wordsets**
- **Create Ai Wordsets**
- **Daily Challenges**


## Built With

- **Backend**: [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet) (with Identity Framework and Entity Framework)
- **Frontend**: [React.js](https://reactjs.org/) + Typescript
- **Containerization**: [Docker](https://www.docker.com/) and Docker Compose

---

## Screenshots

### Main Page
![image](https://github.com/user-attachments/assets/3a9405f5-9cad-4ede-9957-1a2d4f6b4fc7)

### Home Page
![image](https://github.com/user-attachments/assets/aa59a783-9327-4287-832e-9679c81a6d9e)


### Wordset Page
![image](https://github.com/user-attachments/assets/e4834804-040b-4d8b-9696-9d592f521d3d)

---

## Prerequisites

Make sure you have the following installed:

[![Docker][Docker]](https://www.docker.com/)

[![Node.js][Node.js]](https://nodejs.org/)

[![.NET 8 SDK][.NET]](https://dotnet.microsoft.com/)

---

## Start the App

0.: Prepare dotenv file If you're running the app using Docker, go to the main folder. Copy the .env.sample file and rename it to .env. If you're running the app without Docker, go to the LingoLift folder. Copy the .env.sample file and rename it to .env. These .env files contain the necessary environment variables for the app to function properly.


### Using Docker

1. Build docker compose: `docker compose build`

2. Create Frontend .env file, in frontend/vite-project folder, according to the .env.sample Should contain the backend URL

3. Step back, run docker compose: `cd ..`  --> `docker compose up`

4. Access the app: Open your browser, navigate to http://localhost:4000.


### With Terminal

1. Start the database
  
 
3. start the backend server:

  ```sh
    dotnet run
  ```
  - remember the server URL
 
4. Navigate to the frontend directory and create .env according to the .env.sample

  ```sh
    cd ..
    cd frontend/vite-project
  ```
 - .ENV file should contain the backend URL.
 
5. Install npm and start the server

  ```sh
    npm install
    npm run dev
  ```
 
6. Access the app: Open your browser, navigate to http://localhost:4000.

## Contributor

- **Dániel Gőgös**  
  [GitHub Profile](https://github.com/GogosDani)

<!--Links for logos! -->
[Docker]: https://img.shields.io/badge/Docker-blue?style=plastic&logo=docker&logoColor=darkblue
[Node.js]: https://img.shields.io/badge/Node.js-black?style=plastic&logo=nodedotjs&logoColor=green
[.NET]: https://img.shields.io/badge/.NET_8_SDK-darkblue?style=plastic&logo=dotnet&logoColor=white&labelColor=purple



