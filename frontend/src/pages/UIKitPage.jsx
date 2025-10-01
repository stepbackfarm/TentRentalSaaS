import React from 'react';

const UIKitPage = () => {
  return (
    <div className="p-5 font-sans">
      <h1 className="text-4xl font-bold text-blue-800 mb-4">UI Kit - Tent Rental SaaS</h1>
      <p className="text-gray-700 mb-8">This page showcases various UI components for the TentRentalSaaS frontend.</p>

      <section className="mb-10 p-5 border border-gray-200 rounded-lg bg-white">
        <h2 className="text-3xl font-semibold text-blue-800 mb-4">Typography</h2>
        <hr className="my-5 border-gray-300" />
        <h1 className="text-2xl font-bold mb-2">Heading 1 - Event Title</h1>
        <h2 className="text-xl font-semibold mb-2">Heading 2 - Section Title</h2>
        <h3 className="text-lg font-medium mb-2">Heading 3 - Sub-section Title</h3>
        <p className="text-base text-gray-700 leading-relaxed mb-2">Paragraph text. This is a sample paragraph to demonstrate the default text styling. It should be easily readable and convey information clearly.</p>
        <p className="font-bold">Bold text example.</p>
        <p className="italic">Italic text example.</p>
      </section>

      <section className="mb-10 p-5 border border-gray-200 rounded-lg bg-white">
        <h2 className="text-3xl font-semibold text-blue-800 mb-4">Buttons</h2>
        <hr className="my-5 border-gray-300" />
        <div className="flex gap-4 mb-4">
          <button className="px-5 py-2 border-none rounded-md cursor-pointer bg-blue-600 text-white hover:bg-blue-700">Primary Button</button>
          <button className="px-5 py-2 border border-blue-600 rounded-md cursor-pointer bg-white text-blue-600 hover:bg-gray-100">Secondary Button</button>
          <button className="px-5 py-2 border-none rounded-md cursor-pointer bg-red-600 text-white hover:bg-red-700">Danger Button</button>
        </div>
        <div className="flex gap-4">
          <button disabled className="px-5 py-2 border-none rounded-md cursor-not-allowed bg-gray-300 text-gray-600">Disabled Button</button>
        </div>
      </section>

      <section className="mb-10 p-5 border border-gray-200 rounded-lg bg-white">
        <h2 className="text-3xl font-semibold text-blue-800 mb-4">Form Elements</h2>
        <hr className="my-5 border-gray-300" />
        <div className="mb-4">
          <label htmlFor="textInput" className="block mb-1 font-bold text-gray-700">Text Input:</label>
          <input type="text" id="textInput" placeholder="Enter text" className="p-2 rounded-md border border-gray-300 w-full max-w-sm" />
        </div>
        <div className="mb-4">
          <label htmlFor="emailInput" className="block mb-1 font-bold text-gray-700">Email Input:</label>
          <input type="email" id="emailInput" placeholder="Enter email" className="p-2 rounded-md border border-gray-300 w-full max-w-sm" />
        </div>
        <div className="mb-4">
          <label htmlFor="dateInput" className="block mb-1 font-bold text-gray-700">Date Input:</label>
          <input type="date" id="dateInput" className="p-2 rounded-md border border-gray-300 w-full max-w-xs" />
        </div>
        <div className="mb-4">
          <input type="checkbox" id="checkboxInput" className="mr-2" />
          <label htmlFor="checkboxInput" className="text-gray-700">I agree to the terms and conditions</label>
        </div>
      </section>

      <section className="mb-10 p-5 border border-gray-200 rounded-lg bg-white">
        <h2 className="text-3xl font-semibold text-blue-800 mb-4">Calendar Placeholder</h2>
        <hr className="my-5 border-gray-300" />
        <div className="border border-dashed border-gray-400 p-12 text-center bg-gray-50 text-gray-500 rounded-md">
          Calendar Component will go here
        </div>
      </section>

      <section className="mb-10 p-5 border border-gray-200 rounded-lg bg-white">
        <h2 className="text-3xl font-semibold text-blue-800 mb-4">Navigation Placeholder</h2>
        <hr className="my-5 border-gray-300" />
        <nav className="bg-gray-100 p-3 rounded-md flex gap-4">
          <a href="#" className="text-blue-600 hover:underline">Home</a>
          <a href="#" className="text-blue-600 hover:underline">Bookings</a>
          <a href="#" className="text-blue-600 hover:underline">Support</a>
        </nav>
      </section>

      <section className="mb-10 p-5 border border-gray-200 rounded-lg bg-white">
        <h2 className="text-3xl font-semibold text-blue-800 mb-4">Booking Details / Layout Placeholder</h2>
        <hr className="my-5 border-gray-300" />
        <div className="border border-blue-600 p-5 bg-blue-50 rounded-md">
          <h3 className="text-xl font-semibold mb-2">Booking Summary</h3>
          <p className="mb-1"><strong className="font-bold">Event Type:</strong> Wedding</p>
          <p className="mb-1"><strong className="font-bold">Date:</strong> October 26, 2025</p>
          <p className="mb-1"><strong className="font-bold">Location:</strong> Sample Venue, 123 Main St</p>
          <p className="mb-4"><strong className="font-bold">Status:</strong> Confirmed</p>
          <button className="px-4 py-2 border-none rounded-md cursor-pointer bg-green-600 text-white mr-4 hover:bg-green-700">Reschedule</button>
          <button className="px-4 py-2 border-none rounded-md cursor-pointer bg-yellow-500 text-black hover:bg-yellow-600">Cancel</button>
        </div>
      </section>

    </div>
  );
};

export default UIKitPage;
