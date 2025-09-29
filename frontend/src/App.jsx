import {
  createBrowserRouter,
  RouterProvider,
} from "react-router-dom";
import HomePage from "./pages/HomePage";
import ConfirmationPage from "./pages/ConfirmationPage";

const router = createBrowserRouter([
  {
    path: "/",
    element: <HomePage />,
  },
  {
    path: "/confirmation",
    element: <ConfirmationPage />,
  },
]);

function App() {
  return (
    <RouterProvider router={router} />
  )
}

export default App
