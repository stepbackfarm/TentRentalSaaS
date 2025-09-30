import React from 'react';
import { Link } from 'react-router-dom';

function ConfirmationPage() {
  return (
    <div className="min-h-screen bg-gray-900 text-white flex flex-col items-center justify-center p-4">
      <div className="bg-gray-800 p-8 rounded-lg shadow-lg text-center max-w-md w-full">
        <h1 className="text-3xl font-bold text-green-400 mb-4">Booking Confirmed!</h1>
        <p className="text-gray-300 mb-6">Your tent is booked. We've sent a confirmation to your email.</p>
        <Link 
          to="/"
          className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-md transition-colors duration-300"
        >
          Book Another Event
        </Link>
      </div>
    </div>
  );
}

export default ConfirmationPage;
