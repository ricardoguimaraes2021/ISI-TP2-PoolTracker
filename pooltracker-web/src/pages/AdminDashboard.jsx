import { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Users, UserPlus, Droplets, FileText, LogOut, Plus, Minus } from 'lucide-react';
import api from '../services/api';
import { logout } from '../services/auth';
import toast from 'react-hot-toast';

const AdminDashboard = () => {
  const [poolStatus, setPoolStatus] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    fetchStatus();
    const interval = setInterval(fetchStatus, 5000);
    return () => clearInterval(interval);
  }, []);

  const fetchStatus = async () => {
    try {
      const response = await api.get('/api/pool/status');
      setPoolStatus(response.data);
      setLoading(false);
    } catch (error) {
      console.error('Error fetching status:', error);
      setLoading(false);
    }
  };

  const handleEnter = async () => {
    try {
      await api.post('/api/pool/enter');
      toast.success('Entrada registada!');
      fetchStatus();
    } catch (error) {
      toast.error('Erro ao registar entrada');
    }
  };

  const handleExit = async () => {
    try {
      await api.post('/api/pool/exit');
      toast.success('Saída registada!');
      fetchStatus();
    } catch (error) {
      toast.error('Erro ao registar saída');
    }
  };

  const handleLogout = () => {
    logout();
    navigate('/admin/login');
    toast.success('Sessão terminada');
  };

  if (loading) {
    return <div className="p-8">A carregar...</div>;
  }

  const occupancyPercentage = poolStatus
    ? Math.round((poolStatus.currentCount / poolStatus.maxCapacity) * 100)
    : 0;

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm border-b">
        <div className="container mx-auto px-4 py-4 flex justify-between items-center">
          <h1 className="text-2xl font-bold text-gray-800">Painel Administrativo</h1>
          <button
            onClick={handleLogout}
            className="flex items-center gap-2 text-gray-600 hover:text-gray-800"
          >
            <LogOut className="w-5 h-5" />
            Sair
          </button>
        </div>
      </header>

      <div className="container mx-auto px-4 py-8">
        {/* Quick Actions */}
        <div className="bg-white rounded-lg shadow-lg p-6 mb-8">
          <h2 className="text-xl font-semibold text-gray-800 mb-4">Controlo de Lotação</h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="text-center">
              <p className="text-4xl font-bold text-blue-600">{poolStatus?.currentCount || 0}</p>
              <p className="text-gray-600">Pessoas na piscina</p>
            </div>
            <div className="text-center">
              <p className="text-4xl font-bold text-gray-800">
                {poolStatus?.maxCapacity || 120}
              </p>
              <p className="text-gray-600">Capacidade máxima</p>
            </div>
            <div className="text-center">
              <p className="text-4xl font-bold text-green-600">{occupancyPercentage}%</p>
              <p className="text-gray-600">Ocupação</p>
            </div>
          </div>
          <div className="mt-6 flex gap-4 justify-center">
            <button
              onClick={handleEnter}
              disabled={!poolStatus?.isOpen || poolStatus.currentCount >= poolStatus.maxCapacity}
              className="flex items-center gap-2 bg-green-600 text-white px-6 py-3 rounded-lg hover:bg-green-700 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <Plus className="w-5 h-5" />
              Entrou
            </button>
            <button
              onClick={handleExit}
              disabled={poolStatus?.currentCount === 0}
              className="flex items-center gap-2 bg-red-600 text-white px-6 py-3 rounded-lg hover:bg-red-700 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <Minus className="w-5 h-5" />
              Saiu
            </button>
          </div>
        </div>

        {/* Navigation Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <Link
            to="/admin/workers"
            className="bg-white rounded-lg shadow-lg p-6 hover:shadow-xl transition-shadow"
          >
            <div className="flex items-center gap-4">
              <div className="bg-blue-100 p-4 rounded-lg">
                <UserPlus className="w-8 h-8 text-blue-600" />
              </div>
              <div>
                <h3 className="text-xl font-semibold text-gray-800">Trabalhadores</h3>
                <p className="text-gray-600">Gerir trabalhadores e turnos</p>
              </div>
            </div>
          </Link>

          <Link
            to="/admin/water-quality"
            className="bg-white rounded-lg shadow-lg p-6 hover:shadow-xl transition-shadow"
          >
            <div className="flex items-center gap-4">
              <div className="bg-cyan-100 p-4 rounded-lg">
                <Droplets className="w-8 h-8 text-cyan-600" />
              </div>
              <div>
                <h3 className="text-xl font-semibold text-gray-800">Qualidade da Água</h3>
                <p className="text-gray-600">Registar medições</p>
              </div>
            </div>
          </Link>

          <Link
            to="/admin/reports"
            className="bg-white rounded-lg shadow-lg p-6 hover:shadow-xl transition-shadow"
          >
            <div className="flex items-center gap-4">
              <div className="bg-purple-100 p-4 rounded-lg">
                <FileText className="w-8 h-8 text-purple-600" />
              </div>
              <div>
                <h3 className="text-xl font-semibold text-gray-800">Relatórios</h3>
                <p className="text-gray-600">Ver relatórios e estatísticas</p>
              </div>
            </div>
          </Link>
        </div>
      </div>
    </div>
  );
};

export default AdminDashboard;

