import React, { useState, useEffect } from 'react';
import { getAvailability } from '../services/api';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';

function Calendar({ onDateSelect }) {
  const [startDate, setStartDate] = useState(null);
  const [endDate, setEndDate] = useState(null);
  const [bookedDates, setBookedDates] = useState([]);

  useEffect(() => {
    const fetchBookedDates = async () => {
      try {
        const today = new Date();
        const sixtyDaysLater = new Date();
        sixtyDaysLater.setDate(today.getDate() + 60);
        const dates = await getAvailability(today.toISOString().split('T')[0], sixtyDaysLater.toISOString().split('T')[0]);
        setBookedDates(dates.map(dateStr => new Date(dateStr)));
      } catch (error) {
        console.error('Failed to fetch booked dates:', error);
      }
    };
    fetchBookedDates();
  }, []);

  const handleDateChange = (dates) => {
    const [start, end] = dates;
    setStartDate(start);
    setEndDate(end);
    if (start && end) {
      onDateSelect({ startDate: start, endDate: end });
    }
  };

  return (
    <div className="flex flex-col items-center w-full">
      <DatePicker
        selected={startDate}
        onChange={handleDateChange}
        startDate={startDate}
        endDate={endDate}
        selectsRange
        filterDate={(date) => !bookedDates.some(d => d.getTime() === date.getTime())}
        minDate={new Date()}
        inline // Show the calendar inline
        calendarClassName="bg-gray-800 border-blue-400 text-white rounded-lg shadow-lg"
        dayClassName={(date) =>
          `mx-1 my-1 p-2 rounded-full text-center cursor-pointer transition-colors duration-200 ` +
          (bookedDates.some(d => d.getTime() === date.getTime())
            ? 'bg-red-600 text-gray-300 cursor-not-allowed'
            : 'hover:bg-blue-600')
        }
        monthClassName={() => 'bg-gray-700'}
        headerClassName="bg-gray-700 p-2 rounded-t-lg"
        className="w-full bg-gray-700 text-white p-2 rounded-md border border-gray-600 focus:ring-blue-500 focus:border-blue-500"
        popperClassName="z-20"
        renderCustomHeader={({ date, decreaseMonth, increaseMonth }) => (
          <div className="flex justify-between items-center text-white">
            <button onClick={decreaseMonth} className="p-2 rounded-full hover:bg-gray-600">
              {'<'}
            </button>
            <span className="text-lg font-semibold">
              {date.toLocaleString('default', { month: 'long', year: 'numeric' })}
            </span>
            <button onClick={increaseMonth} className="p-2 rounded-full hover:bg-gray-600">
              {'>'}
            </button>
          </div>
        )}
      />
    </div>
  );
}

export default Calendar;
