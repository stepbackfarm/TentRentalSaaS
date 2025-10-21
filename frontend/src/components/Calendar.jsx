import React, { useState, useEffect, useMemo } from 'react';
import { getAvailability } from '../services/api';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import { normalizeDate } from '../helpers/utils';

function Calendar({ onDateSelect, tentType }) {
  const [startDate, setStartDate] = useState(null);
  const [endDate, setEndDate] = useState(null);
  const [unavailableDates, setUnavailableDates] = useState(new Set());

  useEffect(() => {
    const fetchUnavailableDates = async () => {
      if (!tentType) return;
      try {
        const today = new Date();
        const sixtyDaysLater = new Date();
        sixtyDaysLater.setDate(today.getDate() + 60);

        const dates = await getAvailability(tentType, today.toISOString(), sixtyDaysLater.toISOString());

        const normalizedDates = new Set(
          dates.map(dateStr => normalizeDate(new Date(dateStr)).getTime())
        );
        setUnavailableDates(normalizedDates);
      } catch (error) {
        console.error('Failed to fetch unavailable dates:', error);
      }
    };
    fetchUnavailableDates();
  }, [tentType]);

  const handleDateChange = (dates) => {
    const [start, end] = dates;
    setStartDate(start);
    setEndDate(end);
    if (start && end) {
      onDateSelect({ startDate: start, endDate: end });
    }
  };

  const isDateUnavailable = (date) => {
    return unavailableDates.has(normalizeDate(date).getTime());
  };

  const dayClassName = (date) => {
    const baseClasses = 'mx-1 my-1 p-2 rounded-full text-center cursor-pointer transition-colors duration-200';
    if (isDateUnavailable(date)) {
      return `${baseClasses} bg-red-600 text-gray-300 cursor-not-allowed`;
    }
    return `${baseClasses} hover:bg-blue-600`;
  };

  const calendarKey = useMemo(() => tentType + unavailableDates.size, [tentType, unavailableDates]);

  return (
    <div className="flex flex-col items-center w-full" key={calendarKey}>
      <DatePicker
        selected={startDate}
        onChange={handleDateChange}
        startDate={startDate}
        endDate={endDate}
        selectsRange
        filterDate={(date) => !isDateUnavailable(date)}
        minDate={new Date()}
        inline
        calendarClassName="bg-gray-800 border-blue-400 text-white rounded-lg shadow-lg"
        dayClassName={dayClassName}
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
