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
import LoginPage from "./pages/LoginPage";
import PortalLoginPage from "./pages/PortalLoginPage";
import PortalPage from "./pages/PortalPage";

import MainLayout from "./layouts/MainLayout";

const router = createBrowserRouter([
  {
    element: <MainLayout />,
    children: [
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
      {
        path: "/portal/login-request",
        element: <LoginPage />,
      },
      {
        path: "/portal/login",
        element: <PortalLoginPage />,
      },
      {
        path: "/portal",
        element: <PortalPage />,
      },
    ]
  }
]);

function App() {
  return (
    <RouterProvider router={router} />
  )
}

export default App
