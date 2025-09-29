import React, { useState, useEffect } from 'react';
import { getAvailability } from '../services/api';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';

function Calendar({ onDateSelect }) {
  const [selectedDate, setSelectedDate] = useState(null);
  const [bookedDates, setBookedDates] = useState([]);

  useEffect(() => {
    const fetchBookedDates = async () => {
      try {
        const today = new Date();
        const thirtyDaysLater = new Date();
        thirtyDaysLater.setDate(today.getDate() + 30);
        const dates = await getAvailability(today.toISOString().split('T')[0], thirtyDaysLater.toISOString().split('T')[0]);
        setBookedDates(dates.map(dateStr => new Date(dateStr)));
      } catch (error) {
        console.error('Failed to fetch booked dates:', error);
      }
    };
    fetchBookedDates();
  }, []);

  const isDateBooked = (date) => {
    return bookedDates.some(bookedDate =>
      bookedDate.getFullYear() === date.getFullYear() &&
      bookedDate.getMonth() === date.getMonth() &&
      bookedDate.getDate() === date.getDate()
    );
  };

  const handleDateChange = (date) => {
    setSelectedDate(date);
    onDateSelect(date);
  };

  return (
    <div>
      <h2>Select Event Date</h2>
      <DatePicker
        selected={selectedDate}
        onChange={handleDateChange}
        dateFormat="yyyy/MM/dd"
        filterDate={(date) => !isDateBooked(date)}
        minDate={new Date()}
        placeholderText="Click to select a date"
      />
    </div>
  );
}

export default Calendar;
