import { createBrowserRouter, RouterProvider } from "react-router-dom";
import ReactDOM from "react-dom/client";
import './index.css'
import FrontPage from "./Pages/FrontPage";

const router = createBrowserRouter([
  {
    path: "/",
    element: <FrontPage />
  }])

const rootElement = document.getElementById("root");
const root = ReactDOM.createRoot(rootElement as HTMLElement);
root.render(
  <RouterProvider router={router} />
);
