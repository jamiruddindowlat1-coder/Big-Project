import React, { useState, useEffect } from 'react';
import './Dashboard.css';

function Dashboard() {
  const [user, setUser] = useState(null);
  const [courses, setCourses] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (!token) {
      window.location.href = '/';
      return;
    }

    fetchUserData();
    fetchCourses();
  }, []);

  const fetchUserData = async () => {
    try {
      const response = await fetch('http://localhost:5000/api/user', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });
      const data = await response.json();
      setUser(data);
    } catch (error) {
      console.error('Error:', error);
    }
  };

  const fetchCourses = async () => {
    try {
      const response = await fetch('http://localhost:5000/api/courses', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });
      const data = await response.json();
      setCourses(data);
      setLoading(false);
    } catch (error) {
      console.error('Error:', error);
      setLoading(false);
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    window.location.href = '/';
  };

  if (loading) {
    return (
      <div className="loading-container">
        <div className="spinner"></div>
        <p>লোড হচ্ছে...</p>
      </div>
    );
  }

  return (
    <div className="dashboard">
      <nav className="navbar">
        <div className="navbar-brand">
          <h2>Course Management System</h2>
        </div>
        <div className="navbar-menu">
          <span className="user-name">স্বাগতম, {user?.name || 'ইউজার'}!</span>
          <button onClick={handleLogout} className="logout-btn">লগআউট</button>
        </div>
      </nav>

      <div className="dashboard-container">
        <aside className="sidebar">
          <ul className="menu">
            <li className="active">
              <i className="icon">📚</i>
              <span>সকল কোর্স</span>
            </li>
            <li>
              <i className="icon">📖</i>
              <span>আমার কোর্স</span>
            </li>
            <li>
              <i className="icon">➕</i>
              <span>নতুন কোর্স</span>
            </li>
            <li>
              <i className="icon">👥</i>
              <span>শিক্ষার্থীরা</span>
            </li>
            <li>
              <i className="icon">⚙️</i>
              <span>সেটিংস</span>
            </li>
          </ul>
        </aside>

        <main className="main-content">
          <div className="content-header">
            <h1>সকল কোর্স</h1>
            <button className="add-course-btn">+ নতুন কোর্স যোগ করুন</button>
          </div>

          <div className="courses-grid">
            {courses.length > 0 ? (
              courses.map((course) => (
                <div key={course.id} className="course-card">
                  <div className="course-image">
                    <img src={course.image || 'https://via.placeholder.com/300x200'} alt={course.title} />
                  </div>
                  <div className="course-content">
                    <h3>{course.title}</h3>
                    <p>{course.description}</p>
                    <div className="course-meta">
                      <span className="students">👥 {course.students || 0} জন</span>
                      <span className="duration">⏱️ {course.duration || 'N/A'}</span>
                    </div>
                    <div className="course-footer">
                      <span className="price">৳{course.price || 'ফ্রি'}</span>
                      <button className="view-btn">বিস্তারিত</button>
                    </div>
                  </div>
                </div>
              ))
            ) : (
              <div className="empty-state">
                <div className="empty-icon">📚</div>
                <h3>কোনো কোর্স পাওয়া যায়নি</h3>
                <p>নতুন কোর্স যোগ করুন</p>
                <button className="add-course-btn">+ নতুন কোর্স</button>
              </div>
            )}
          </div>
        </main>
      </div>
    </div>
  );
}

export default Dashboard;