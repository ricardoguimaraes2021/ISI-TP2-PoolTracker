import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { ArrowLeft, FileText, AlertCircle } from 'lucide-react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import api from '../services/api';
import toast from 'react-hot-toast';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Button } from '../components/ui/button';
import { Badge } from '../components/ui/badge';

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
            
            // Format date
            let formattedDate = '';
            if (dateValue) {
              // If it's already a string in format YYYY-MM-DD
              if (typeof dateValue === 'string' && dateValue.length === 10) {
                formattedDate = dateValue;
              } else {
                try {
                  formattedDate = new Date(dateValue).toLocaleDateString('pt-PT');
                } catch (e) {
                  formattedDate = dateValue.toString();
                }
              }
            }

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
    <div className="min-h-screen bg-background">
      <div className="container mx-auto px-4 py-8">
        <Link to="/admin" className="mb-8 inline-block">
          <Button variant="ghost" className="flex items-center gap-2">
            <ArrowLeft className="w-5 h-5" />
            Voltar
          </Button>
        </Link>

        <h1 className="text-3xl font-bold text-foreground mb-8">Relatórios e Estatísticas</h1>

        {error && (
          <Card className="border-destructive/50 bg-destructive/10 mb-6">
            <CardContent className="pt-6">
              <div className="flex items-center gap-2 text-destructive">
                <AlertCircle className="w-5 h-5" />
                <span>{error}</span>
              </div>
            </CardContent>
          </Card>
        )}

        {statistics && Array.isArray(statistics) && statistics.length > 0 && (
          <Card className="mb-8">
            <CardHeader>
              <CardTitle>Fluxo de Visitantes (7 dias)</CardTitle>
            </CardHeader>
            <CardContent>
            <ResponsiveContainer width="100%" height={300}>
              <LineChart data={statistics}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="date" />
                <YAxis />
                <Tooltip />
                <Line type="monotone" dataKey="visitors" stroke="#3b82f6" strokeWidth={2} />
              </LineChart>
            </ResponsiveContainer>
            </CardContent>
          </Card>
        )}

        {!statistics && !loading && (
          <Card className="mb-6 border-yellow-500/50 bg-yellow-500/10">
            <CardContent className="pt-6">
              <p className="text-yellow-700 dark:text-yellow-400">Não há dados de estatísticas disponíveis.</p>
            </CardContent>
          </Card>
        )}

        <div className="space-y-4">
          {reports && reports.length > 0 ? (
            reports.map((report) => (
              <Card key={report.id}>
                <CardHeader>
                  <div className="flex items-center gap-3">
                    <FileText className="w-6 h-6 text-purple-500" />
                    <CardTitle>
                      Relatório - {report.reportDate ? new Date(report.reportDate).toLocaleDateString('pt-PT') : 'Data não disponível'}
                    </CardTitle>
                  </div>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                    <div>
                      <p className="text-sm text-muted-foreground">Total Visitantes</p>
                      <p className="text-2xl font-bold text-foreground">{report.totalVisitors ?? 0}</p>
                    </div>
                    <div>
                      <p className="text-sm text-muted-foreground">Ocupação Máxima</p>
                      <p className="text-2xl font-bold text-foreground">{report.maxOccupancy ?? 0}</p>
                    </div>
                    <div>
                      <p className="text-sm text-muted-foreground">Ocupação Média</p>
                      <p className="text-2xl font-bold text-foreground">
                        {report.avgOccupancy ? report.avgOccupancy.toFixed(1) : 'N/A'}
                      </p>
                    </div>
                    <div>
                      <p className="text-sm text-muted-foreground">Hora de Fecho</p>
                      <p className="text-lg font-semibold text-foreground">
                        {report.closingTime ? new Date(report.closingTime).toLocaleTimeString('pt-PT') : 'N/A'}
                      </p>
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))
          ) : (
            <Card>
              <CardContent className="pt-6 text-center">
                <FileText className="w-12 h-12 text-muted-foreground mx-auto mb-4" />
                <p className="text-muted-foreground">Não há relatórios disponíveis.</p>
              </CardContent>
            </Card>
          )}
        </div>
      </div>
    </div>
  );
};

export default ReportsPage;

