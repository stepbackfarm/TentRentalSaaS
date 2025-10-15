import React from 'react';

function GalleryPage() {
  const images = [
    "/tent-with-dimensions.jpg",
    "https://via.placeholder.com/400x300.png?text=Tent+at+a+Birthday+Party",
    "https://via.placeholder.com/400x300.png?text=Tent+at+a+Corporate+Event",
    "https://via.placeholder.com/400x300.png?text=Tent+at+a+Family+Reunion",
    "https://via.placeholder.com/400x300.png?text=Tent+at+a+Community+Festival",
    "https://via.placeholder.com/400x300.png?text=Tent+Interior+Decorated",
    "https://via.placeholder.com/400x300.png?text=Tent+Exterior+at+Night",
    "https://via.placeholder.com/400x300.png?text=Happy+Guests+Under+the+Tent"
  ];

  return (
    <div className="min-h-screen bg-gray-900 text-white flex flex-col items-center p-4 sm:p-8">
      <div className="w-full max-w-6xl bg-gray-800 p-6 sm:p-8 rounded-lg shadow-lg">
        <h1 className="text-3xl sm:text-4xl font-bold text-center text-blue-400 mb-6 sm:mb-8">Gallery</h1>
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
          {images.map((src, index) => (
            <div key={index} className="bg-gray-700 p-2 rounded-lg">
              <img src={src} alt={`Gallery image ${index + 1}`} className="w-full h-auto rounded-lg"/>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

export default GalleryPage;
