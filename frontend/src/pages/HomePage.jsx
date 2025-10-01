import React from 'react';
import { useNavigate } from 'react-router-dom';
import Calendar from '../components/Calendar';

function HomePage() {
  const navigate = useNavigate();

  const handleDateSelect = (date) => {
    navigate('/checkout', { state: { selectedDate: date.toISOString() } });
  };

  return (
    <div className="min-h-screen bg-gray-900 text-white flex flex-col items-center p-4 sm:p-8">
      <div className="w-full max-w-4xl bg-gray-800 p-6 sm:p-8 rounded-lg shadow-lg">
        <h1 className="text-3xl sm:text-4xl font-bold text-center text-blue-400 mb-6 sm:mb-8">Tent Rental Booking</h1>
        <div className="flex justify-center bg-gray-700 p-4 rounded-lg">
          <div>
            <h2 className="text-xl font-semibold text-center mb-4">Select a Date</h2>
            <Calendar onDateSelect={handleDateSelect} />
          </div>
        </div>
      </div>
    </div>
  );
}

export default HomePage;
