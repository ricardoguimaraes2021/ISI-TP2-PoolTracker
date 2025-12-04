import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { ArrowLeft, FileText, AlertCircle } from 'lucide-react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import api from '../services/api';
import toast from 'react-hot-toast';

const ReportsPage = () => {
  const [reports, setReports] = useState([]);
  const [statistics, setStatistics] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setError(null);
      const [reportsRes, statsRes] = await Promise.allSettled([
        api.get('/api/reports'),
        api.get('/api/statistics/visitors'),
      ]);

      // Handle reports response
      if (reportsRes.status === 'fulfilled') {
        setReports(Array.isArray(reportsRes.value.data) ? reportsRes.value.data : []);
      } else {
        console.error('Error fetching reports:', reportsRes.reason);
        setReports([]);
      }

      // Handle statistics response
      if (statsRes.status === 'fulfilled') {
        const statsData = statsRes.value.data;
        // VisitorsStatisticsDto has a Data property with array of VisitorDataPoint
        // Handle both camelCase (data) and PascalCase (Data)
        const dataArray = statsData?.data || statsData?.Data;
        if (dataArray && Array.isArray(dataArray) && dataArray.length > 0) {
          // Transform DateOnly to string for chart
          const chartData = dataArray.map(item => {
            const dateValue = item.date || item.Date || '';
            const visitorsValue = item.visitors ?? item.Visitors ?? 0;
            // Format date if it's a DateOnly string (YYYY-MM-DD)
            const formattedDate = dateValue ? (dateValue.length === 10 ? dateValue : new Date(dateValue).toLocaleDateString('pt-PT')) : '';
            return {
              date: formattedDate,
              visitors: visitorsValue
            };
          });
          setStatistics(chartData);
        } else {
          setStatistics(null);
        }
      } else {
        console.error('Error fetching statistics:', statsRes.reason);
        setStatistics(null);
      }

      setLoading(false);
    } catch (error) {
      console.error('Error fetching data:', error);
      setError('Erro ao carregar dados. Por favor, tente novamente.');
      setLoading(false);
      toast.error('Erro ao carregar relatórios');
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50">
        <div className="container mx-auto px-4 py-8">
          <div className="p-8 text-center">A carregar...</div>
        </div>
      </div>
    );
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

        {error && (
          <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg mb-6 flex items-center gap-2">
            <AlertCircle className="w-5 h-5" />
            <span>{error}</span>
          </div>
        )}

        {statistics && Array.isArray(statistics) && statistics.length > 0 && (
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

        {!statistics && !loading && (
          <div className="bg-yellow-50 border border-yellow-200 text-yellow-700 px-4 py-3 rounded-lg mb-6">
            Não há dados de estatísticas disponíveis.
          </div>
        )}

        <div className="space-y-4">
          {reports && reports.length > 0 ? (
            reports.map((report) => (
              <div key={report.id} className="bg-white rounded-lg shadow-lg p-6">
                <div className="flex items-center gap-3 mb-4">
                  <FileText className="w-6 h-6 text-purple-600" />
                  <h3 className="text-lg font-semibold text-gray-800">
                    Relatório - {report.reportDate ? new Date(report.reportDate).toLocaleDateString('pt-PT') : 'Data não disponível'}
                  </h3>
                </div>
                <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                  <div>
                    <p className="text-sm text-gray-600">Total Visitantes</p>
                    <p className="text-2xl font-bold text-gray-800">{report.totalVisitors ?? 0}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-600">Ocupação Máxima</p>
                    <p className="text-2xl font-bold text-gray-800">{report.maxOccupancy ?? 0}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-600">Ocupação Média</p>
                    <p className="text-2xl font-bold text-gray-800">
                      {report.avgOccupancy ? report.avgOccupancy.toFixed(1) : 'N/A'}
                    </p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-600">Hora de Fecho</p>
                    <p className="text-lg font-semibold text-gray-800">
                      {report.closingTime ? new Date(report.closingTime).toLocaleTimeString('pt-PT') : 'N/A'}
                    </p>
                  </div>
                </div>
              </div>
            ))
          ) : (
            <div className="bg-white rounded-lg shadow-lg p-6 text-center">
              <FileText className="w-12 h-12 text-gray-400 mx-auto mb-4" />
              <p className="text-gray-600">Não há relatórios disponíveis.</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default ReportsPage;

