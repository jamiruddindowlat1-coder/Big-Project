import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { useState } from "react";

import AuthPage from "./components/AuthPage";
import Sidebar from "./components/Sidebar";

import Dashboard from "./pages/Dashboard";
import Courses from "./pages/Courses";
import Students from "./pages/Students";
import Teachers from "./pages/Teachers";
import Enrollments from "./pages/Enrollments";

function App() {
  const [isAuth, setIsAuth] = useState(
    !!localStorage.getItem("token")
  );

  return (
    <BrowserRouter>
      {!isAuth ? (
        <Routes>
          <Route path="*" element={<AuthPage setIsAuth={setIsAuth} />} />
        </Routes>
      ) : (
        <>
          <Sidebar setIsAuth={setIsAuth} />

          <div style={{ marginLeft: "220px", padding: "20px" }}>
            <Routes>
              <Route path="/" element={<Dashboard />} />
              <Route path="/courses" element={<Courses />} />
              <Route path="/students" element={<Students />} />
              <Route path="/teachers" element={<Teachers />} />
              <Route path="/enrollments" element={<Enrollments />} />
              <Route path="*" element={<Navigate to="/" />} />
            </Routes>
          </div>
        </>
      )}
    </BrowserRouter>
  );
}

export default App;
