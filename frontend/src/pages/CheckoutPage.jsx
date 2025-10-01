import React from 'react';
import { useLocation } from 'react-router-dom';
import BookingForm from '../components/BookingForm';
import { Elements } from '@stripe/react-stripe-js';
import { loadStripe } from '@stripe/stripe-js';

const stripePromise = loadStripe(import.meta.env.VITE_STRIPE_PUBLISHABLE_KEY);

function CheckoutPage() {
  const location = useLocation();
  const selectedDate = location.state?.selectedDate;

  return (
    <div className="min-h-screen bg-gray-900 text-white flex flex-col items-center p-4 sm:p-8">
      <div className="w-full max-w-xl bg-gray-800 p-6 sm:p-8 rounded-lg shadow-lg">
        <h1 className="text-3xl sm:text-4xl font-bold text-center text-blue-400 mb-6 sm:mb-8">Confirm Your Booking</h1>
        <Elements stripe={stripePromise}>
          {selectedDate ? (
            <BookingForm selectedDate={new Date(selectedDate)} />
          ) : (
            <p className="text-center text-red-400">No date selected. Please go back and select a date.</p>
          )}
        </Elements>
      </div>
    </div>
  );
}

export default CheckoutPage;
