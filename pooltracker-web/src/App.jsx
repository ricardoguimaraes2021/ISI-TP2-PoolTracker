import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import PublicPage from './pages/PublicPage';
import AdminLogin from './pages/AdminLogin';
import AdminDashboard from './pages/AdminDashboard';
import WorkersPage from './pages/WorkersPage';
import WaterQualityPage from './pages/WaterQualityPage';
import ReportsPage from './pages/ReportsPage';
import ProtectedRoute from './components/ProtectedRoute';

function App() {
  return (
    <Router>
      <div className="min-h-screen bg-background">
        <Toaster 
          position="top-right"
          toastOptions={{
            className: "bg-card border border-border",
            style: {
              background: "hsl(var(--card))",
              color: "hsl(var(--card-foreground))",
              border: "1px solid hsl(var(--border))",
            },
          }}
        />
        <Routes>
          <Route path="/" element={<PublicPage />} />
          <Route path="/admin/login" element={<AdminLogin />} />
          <Route
            path="/admin"
            element={
              <ProtectedRoute>
                <AdminDashboard />
              </ProtectedRoute>
            }
          />
          {/* Rotas antigas redirecionam para AdminDashboard com tab correta */}
          <Route
            path="/admin/workers"
            element={
              <ProtectedRoute>
                <AdminDashboard initialTab="workers" />
              </ProtectedRoute>
            }
          />
          <Route
            path="/admin/water-quality"
            element={
              <ProtectedRoute>
                <AdminDashboard initialTab="water" />
              </ProtectedRoute>
            }
          />
          <Route
            path="/admin/reports"
            element={
              <ProtectedRoute>
                <AdminDashboard initialTab="reports" />
              </ProtectedRoute>
            }
          />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
