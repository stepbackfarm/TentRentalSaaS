import React, { useState } from 'react';
import Calendar from '../components/Calendar';
import BookingForm from '../components/BookingForm';

function HomePage() {
  const [selectedDate, setSelectedDate] = useState(null);

  const handleDateSelect = (date) => {
    setSelectedDate(date);
  };

  return (
    <div>
      <h1>Tent Rental Booking</h1>
      <Calendar onDateSelect={handleDateSelect} />
      {selectedDate && <BookingForm selectedDate={selectedDate} />}
    </div>
  );
}

export default HomePage;
