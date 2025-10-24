import { Link, useLocation } from "react-router";

export function NavBar() {
  const location = useLocation();

  const isActive = (path: string) => {
    if (path === "/") {
      return location.pathname === "/";
    }
    return location.pathname.startsWith(path);
  };

  return (
    <nav className="bg-blue-600 text-white shadow-lg">
      <div className="max-w-7xl mx-auto px-4">
        <div className="flex justify-between items-center h-16">
          <div className="flex items-center space-x-8">
            <Link to="/" className="text-xl font-bold hover:text-blue-200">
              Sports Ticker
            </Link>
            
            <div className="flex space-x-4">
              <Link
                to="/"
                className={`px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                  isActive("/") 
                    ? "bg-blue-700 text-white" 
                    : "text-blue-100 hover:bg-blue-500 hover:text-white"
                }`}
              >
                Home
              </Link>
              
              <Link
                to="/ticker/sports-ticker"
                className={`px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                  isActive("/ticker") 
                    ? "bg-blue-700 text-white" 
                    : "text-blue-100 hover:bg-blue-500 hover:text-white"
                }`}
              >
                Live Ticker
              </Link>
            </div>
          </div>
        </div>
      </div>
    </nav>
  );
}
