import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { ArrowLeft, FileText } from 'lucide-react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import api from '../services/api';

const ReportsPage = () => {
  const [reports, setReports] = useState([]);
  const [statistics, setStatistics] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      const [reportsRes, statsRes] = await Promise.all([
        api.get('/api/reports'),
        api.get('/api/statistics/visitors'),
      ]);
      setReports(reportsRes.data);
      setStatistics(statsRes.data);
      setLoading(false);
    } catch (error) {
      console.error('Error fetching data:', error);
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="p-8">A carregar...</div>;
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="container mx-auto px-4 py-8">
        <Link
          to="/admin"
          className="inline-flex items-center gap-2 text-gray-600 hover:text-gray-800 mb-8"
        >
          <ArrowLeft className="w-5 h-5" />
          Voltar
        </Link>

        <h1 className="text-3xl font-bold text-gray-800 mb-8">Relatórios e Estatísticas</h1>

        {statistics && (
          <div className="bg-white rounded-lg shadow-lg p-6 mb-8">
            <h2 className="text-xl font-semibold mb-4">Fluxo de Visitantes (7 dias)</h2>
            <ResponsiveContainer width="100%" height={300}>
              <LineChart data={statistics}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="date" />
                <YAxis />
                <Tooltip />
                <Line type="monotone" dataKey="visitors" stroke="#3b82f6" strokeWidth={2} />
              </LineChart>
            </ResponsiveContainer>
          </div>
        )}

        <div className="space-y-4">
          {reports.map((report) => (
            <div key={report.id} className="bg-white rounded-lg shadow-lg p-6">
              <div className="flex items-center gap-3 mb-4">
                <FileText className="w-6 h-6 text-purple-600" />
                <h3 className="text-lg font-semibold text-gray-800">
                  Relatório - {new Date(report.reportDate).toLocaleDateString('pt-PT')}
                </h3>
              </div>
              <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                <div>
                  <p className="text-sm text-gray-600">Total Visitantes</p>
                  <p className="text-2xl font-bold text-gray-800">{report.totalVisitors}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Ocupação Máxima</p>
                  <p className="text-2xl font-bold text-gray-800">{report.maxOccupancy}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Ocupação Média</p>
                  <p className="text-2xl font-bold text-gray-800">
                    {report.avgOccupancy?.toFixed(1) || 'N/A'}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Hora de Fecho</p>
                  <p className="text-lg font-semibold text-gray-800">
                    {new Date(report.closingTime).toLocaleTimeString('pt-PT')}
                  </p>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default ReportsPage;

