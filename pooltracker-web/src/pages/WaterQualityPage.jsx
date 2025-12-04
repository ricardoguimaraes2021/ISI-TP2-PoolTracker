import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { ArrowLeft, Plus, Droplets } from 'lucide-react';
import api from '../services/api';
import toast from 'react-hot-toast';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Select } from '../components/ui/select';

const WaterQualityPage = () => {
  const [measurements, setMeasurements] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [formData, setFormData] = useState({
    poolType: 'Criancas',
    phLevel: '',
    temperature: '',
    notes: '',
  });

  useEffect(() => {
    fetchMeasurements();
  }, []);

  const fetchMeasurements = async () => {
    try {
      const response = await api.get('/api/water-quality');
      setMeasurements(response.data);
      setLoading(false);
    } catch (error) {
      console.error('Error fetching measurements:', error);
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await api.post('/api/water-quality', {
        ...formData,
        phLevel: parseFloat(formData.phLevel),
        temperature: parseFloat(formData.temperature),
      });
      toast.success('Medição registada com sucesso!');
      setShowForm(false);
      setFormData({ poolType: 'Criancas', phLevel: '', temperature: '', notes: '' });
      fetchMeasurements();
    } catch (error) {
      toast.error('Erro ao registar medição');
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-background flex items-center justify-center">
        <div className="text-foreground">A carregar...</div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-background">
      <div className="container mx-auto px-4 py-8">
        <div className="flex items-center justify-between mb-8">
          <Link to="/admin">
            <Button variant="ghost" className="flex items-center gap-2">
              <ArrowLeft className="w-5 h-5" />
              Voltar
            </Button>
          </Link>
          <Button onClick={() => setShowForm(!showForm)}>
            <Plus className="w-5 h-5 mr-2" />
            Nova Medição
          </Button>
        </div>

        {showForm && (
          <Card className="mb-6">
            <CardHeader>
              <CardTitle>Registar Medição</CardTitle>
            </CardHeader>
            <CardContent>
              <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-foreground mb-2">
                    Tipo de Piscina
                  </label>
                  <Select
                    value={formData.poolType}
                    onChange={(e) => setFormData({ ...formData, poolType: e.target.value })}
                  >
                    <option value="Criancas">Crianças</option>
                    <option value="Adultos">Adultos</option>
                  </Select>
                </div>
                <div>
                  <label className="block text-sm font-medium text-foreground mb-2">pH</label>
                  <Input
                    type="number"
                    step="0.1"
                    value={formData.phLevel}
                    onChange={(e) => setFormData({ ...formData, phLevel: e.target.value })}
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-foreground mb-2">
                    Temperatura (°C)
                  </label>
                  <Input
                    type="number"
                    step="0.1"
                    value={formData.temperature}
                    onChange={(e) => setFormData({ ...formData, temperature: e.target.value })}
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-foreground mb-2">Notas</label>
                  <textarea
                    value={formData.notes}
                    onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
                    className="flex min-h-[80px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
                    rows="3"
                  />
                </div>
                <div className="flex gap-4">
                  <Button type="submit">Registar</Button>
                  <Button type="button" variant="outline" onClick={() => setShowForm(false)}>
                    Cancelar
                  </Button>
                </div>
              </form>
            </CardContent>
          </Card>
        )}

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {measurements.map((measurement) => (
            <Card key={measurement.id}>
              <CardHeader>
                <div className="flex items-center gap-3">
                  <Droplets className="w-8 h-8 text-cyan-500" />
                  <div>
                    <CardTitle className="text-lg">
                      Piscina {measurement.poolType}
                    </CardTitle>
                    <p className="text-sm text-muted-foreground">
                      {new Date(measurement.measuredAt).toLocaleString('pt-PT')}
                    </p>
                  </div>
                </div>
              </CardHeader>
              <CardContent>
                <div className="space-y-2">
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">pH:</span>
                    <span className="font-semibold">{measurement.phLevel}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Temperatura:</span>
                    <span className="font-semibold">{measurement.temperature}°C</span>
                  </div>
                  {measurement.notes && (
                    <div className="mt-4 pt-4 border-t border-border">
                      <p className="text-sm text-muted-foreground">{measurement.notes}</p>
                    </div>
                  )}
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      </div>
    </div>
  );
};

export default WaterQualityPage;
