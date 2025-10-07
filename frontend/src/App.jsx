import {
  createBrowserRouter,
  RouterProvider,
} from "react-router-dom";
import HomePage from "./pages/HomePage";
import ConfirmationPage from "./pages/ConfirmationPage";
import UIKitPage from "./pages/UIKitPage";
import CheckoutPage from "./pages/CheckoutPage";
import FaqPage from "./pages/FaqPage";
import GalleryPage from "./pages/GalleryPage";

const router = createBrowserRouter([
  {
    path: "/",
    element: <HomePage />,
  },
  {
    path: "/confirmation",
    element: <ConfirmationPage />,
  },
  {
    path: "/uikit",
    element: <UIKitPage />,
  },
  {
    path: "/checkout",
    element: <CheckoutPage />,
  },
  {
    path: "/faq",
    element: <FaqPage />,
  },
  {
    path: "/gallery",
    element: <GalleryPage />,
  },
]);

function App() {
  return (
    <RouterProvider router={router} />
  )
}

export default App
