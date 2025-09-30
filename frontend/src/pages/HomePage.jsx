import React, { useState } from 'react';
import Calendar from '../components/Calendar';
import BookingForm from '../components/BookingForm';

function HomePage() {
  const [selectedDate, setSelectedDate] = useState(null);

  const handleDateSelect = (date) => {
    setSelectedDate(date);
  };

  return (
    <div className="min-h-screen bg-gray-900 text-white flex flex-col items-center p-4 sm:p-8">
      <div className="w-full max-w-4xl bg-gray-800 p-6 sm:p-8 rounded-lg shadow-lg">
        <h1 className="text-3xl sm:text-4xl font-bold text-center text-blue-400 mb-6 sm:mb-8">Tent Rental Booking</h1>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6 sm:gap-8">
          <div className="bg-gray-700 p-4 rounded-lg">
            <h2 className="text-xl font-semibold text-center mb-4">1. Select a Date</h2>
            <Calendar onDateSelect={handleDateSelect} />
          </div>
          <div className="bg-gray-700 p-4 rounded-lg">
            <h2 className="text-xl font-semibold text-center mb-4">2. Enter Your Details</h2>
            {selectedDate ? (
              <BookingForm selectedDate={selectedDate} />
            ) : (
              <div className="flex items-center justify-center h-full">
                <p className="text-gray-400">Please select a date to see the booking form.</p>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

export default HomePage;
