import { createBrowserRouter, RouterProvider } from "react-router-dom";
import ReactDOM from "react-dom/client";
import './index.css'
import FrontPage from "./Pages/FrontPage";
import LeaderBoard from "./Pages/LeaderBoard";
import DailyChallenge from "./Pages/DailyChallenge";
import Home from "./Pages/Home";

const router = createBrowserRouter([
  {
    path: "/",
    element: <FrontPage />
  },
  {
    path: "/daily-challenge",
    element: <DailyChallenge />
  },
  {
    path: "/leaderboard",
    element: <LeaderBoard />
  },
  {
    path: "/home",
    element: <Home />
  }
])

const rootElement = document.getElementById("root");
const root = ReactDOM.createRoot(rootElement as HTMLElement);
root.render(
  <RouterProvider router={router} />
);
