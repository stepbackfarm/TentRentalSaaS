import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import Calendar from '../components/Calendar';

function HomePage() {
  const navigate = useNavigate();
  const [tentType, setTentType] = useState('20x40 Event Tent');
  const [bookingCount, setBookingCount] = useState(0);

  useEffect(() => {
    const handleBookingCompletion = () => {
      setBookingCount(prev => prev + 1);
    };

    window.addEventListener('bookingCompleted', handleBookingCompletion);

    return () => {
      window.removeEventListener('bookingCompleted', handleBookingCompletion);
    };
  }, []);

  const handleDateSelect = (dateRange) => {
    navigate('/checkout', {
      state: {
        startDate: dateRange.startDate.toISOString(),
        endDate: dateRange.endDate.toISOString(),
        tentType: tentType,
      }
    });
  };

  return (
    <div className="min-h-screen bg-gray-900 text-white flex flex-col items-center p-4 sm:p-8">
      <div className="w-full max-w-4xl bg-gray-800 p-6 sm:p-8 rounded-lg shadow-lg">
        <h1 className="text-3xl sm:text-4xl font-bold text-center text-blue-400 mb-6 sm:mb-8">Tent Rental Booking</h1>

        {/* Tent selection can be added here in the future */}
        {/* For now, we are using the hardcoded '20x40 Event Tent' */}

        <div className="flex justify-center bg-gray-700 p-4 rounded-lg">
          <div>
            <h2 className="text-xl font-semibold text-center mb-4">Select a Date</h2>
            <Calendar
              key={bookingCount}
              onDateSelect={handleDateSelect}
              tentType={tentType}
            />
          </div>
        </div>

        <div className="mt-8">
          <h2 className="text-2xl font-bold text-center text-blue-400 mb-4">Our Tent</h2>
          <div className="bg-gray-700 p-6 rounded-lg">
            <p className="text-lg"><span className="font-semibold">Size:</span> 20x40 feet</p>
            <p className="text-lg"><span className="font-semibold">Capacity:</span> Seats 40-50 people</p>
            <p className="text-lg"><span className="font-semibold">Includes:</span> Tent, setup, and takedown</p>
            <p className="text-lg"><span className="font-semibold">Price:</span> $250 per day</p>
          </div>
        </div>

        <div className="mt-8">
          <h2 className="text-2xl font-bold text-center text-blue-400 mb-4">Testimonials</h2>
          <div className="bg-gray-700 p-6 rounded-lg space-y-4">
            <div>
              <p className="text-lg italic">"The tent was perfect for our party. The team was professional and the setup was quick."</p>
              <p className="text-right font-semibold">- Jane Doe</p>
            </div>
            <div>
              <p className="text-lg italic">"Great service and a great price. I would definitely recommend them to anyone."</p>
              <p className="text-right font-semibold">- John Smith</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default HomePage;
