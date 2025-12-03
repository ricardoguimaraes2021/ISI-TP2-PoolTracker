import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { ArrowLeft, Plus, Droplets } from 'lucide-react';
import api from '../services/api';
import toast from 'react-hot-toast';

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
    return <div className="p-8">A carregar...</div>;
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="container mx-auto px-4 py-8">
        <div className="flex items-center justify-between mb-8">
          <Link
            to="/admin"
            className="flex items-center gap-2 text-gray-600 hover:text-gray-800"
          >
            <ArrowLeft className="w-5 h-5" />
            Voltar
          </Link>
          <button
            onClick={() => setShowForm(!showForm)}
            className="flex items-center gap-2 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700"
          >
            <Plus className="w-5 h-5" />
            Nova Medição
          </button>
        </div>

        {showForm && (
          <div className="bg-white rounded-lg shadow-lg p-6 mb-6">
            <h2 className="text-xl font-semibold mb-4">Registar Medição</h2>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Tipo de Piscina
                </label>
                <select
                  value={formData.poolType}
                  onChange={(e) => setFormData({ ...formData, poolType: e.target.value })}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg"
                >
                  <option value="Criancas">Crianças</option>
                  <option value="Adultos">Adultos</option>
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">pH</label>
                <input
                  type="number"
                  step="0.1"
                  value={formData.phLevel}
                  onChange={(e) => setFormData({ ...formData, phLevel: e.target.value })}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Temperatura (°C)
                </label>
                <input
                  type="number"
                  step="0.1"
                  value={formData.temperature}
                  onChange={(e) => setFormData({ ...formData, temperature: e.target.value })}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">Notas</label>
                <textarea
                  value={formData.notes}
                  onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg"
                  rows="3"
                />
              </div>
              <div className="flex gap-4">
                <button
                  type="submit"
                  className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700"
                >
                  Registar
                </button>
                <button
                  type="button"
                  onClick={() => setShowForm(false)}
                  className="bg-gray-300 text-gray-700 px-6 py-2 rounded-lg hover:bg-gray-400"
                >
                  Cancelar
                </button>
              </div>
            </form>
          </div>
        )}

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {measurements.map((measurement) => (
            <div key={measurement.id} className="bg-white rounded-lg shadow-lg p-6">
              <div className="flex items-center gap-3 mb-4">
                <Droplets className="w-8 h-8 text-cyan-600" />
                <div>
                  <h3 className="text-lg font-semibold text-gray-800">
                    Piscina {measurement.poolType}
                  </h3>
                  <p className="text-sm text-gray-600">
                    {new Date(measurement.measuredAt).toLocaleString('pt-PT')}
                  </p>
                </div>
              </div>
              <div className="space-y-2">
                <div className="flex justify-between">
                  <span className="text-gray-600">pH:</span>
                  <span className="font-semibold">{measurement.phLevel}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Temperatura:</span>
                  <span className="font-semibold">{measurement.temperature}°C</span>
                </div>
                {measurement.notes && (
                  <div className="mt-4 pt-4 border-t">
                    <p className="text-sm text-gray-600">{measurement.notes}</p>
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default WaterQualityPage;

