import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { ArrowLeft, Plus, Edit, Trash2, Clock, X } from 'lucide-react';
import api from '../services/api';
import toast from 'react-hot-toast';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Select } from '../components/ui/select';
import { Badge } from '../components/ui/badge';

const WorkersPage = () => {
  const [workers, setWorkers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editingWorker, setEditingWorker] = useState(null);
  const [formData, setFormData] = useState({ name: '', role: 'nadador_salvador' });
  const [shiftType, setShiftType] = useState('Manha');

  useEffect(() => {
    fetchWorkers();
  }, []);

  const fetchWorkers = async () => {
    try {
      const response = await api.get('/api/workers');
      setWorkers(response.data);
      setLoading(false);
    } catch (error) {
      console.error('Error fetching workers:', error);
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (editingWorker) {
        await api.put(`/api/workers/${editingWorker.workerId}`, {
          name: formData.name,
          role: formData.role
        });
        toast.success('Trabalhador atualizado com sucesso!');
        setEditingWorker(null);
      } else {
        await api.post('/api/workers', formData);
        toast.success('Trabalhador criado com sucesso!');
      }
      setShowForm(false);
      setFormData({ name: '', role: 'nadador_salvador' });
      fetchWorkers();
    } catch (error) {
      toast.error(editingWorker ? 'Erro ao atualizar trabalhador' : 'Erro ao criar trabalhador');
    }
  };

  const handleEdit = (worker) => {
    setEditingWorker(worker);
    setFormData({ name: worker.name, role: worker.role.toLowerCase() });
    setShowForm(true);
  };

  const handleCancelEdit = () => {
    setEditingWorker(null);
    setFormData({ name: '', role: 'nadador_salvador' });
    setShowForm(false);
  };

  const handleDelete = async (workerId) => {
    if (!window.confirm('Tem certeza que deseja eliminar este trabalhador?')) {
      return;
    }
    try {
      await api.delete(`/api/workers/${workerId}`);
      toast.success('Trabalhador eliminado com sucesso!');
      fetchWorkers();
    } catch (error) {
      toast.error('Erro ao eliminar trabalhador');
    }
  };

  const handleActivateShift = async (workerId, shiftType) => {
    try {
      await api.post(`/api/workers/${workerId}/activate`, { shiftType });
      toast.success('Turno ativado!');
      fetchWorkers();
    } catch (error) {
      toast.error('Erro ao ativar turno');
    }
  };

  const handleDeactivateShift = async (workerId) => {
    try {
      await api.post(`/api/workers/${workerId}/deactivate`);
      toast.success('Turno desativado!');
      fetchWorkers();
    } catch (error) {
      toast.error('Erro ao desativar turno');
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
          <Button onClick={() => {
            setEditingWorker(null);
            setFormData({ name: '', role: 'nadador_salvador' });
            setShowForm(!showForm);
          }}>
            <Plus className="w-5 h-5 mr-2" />
            Novo Trabalhador
          </Button>
        </div>

        {showForm && (
          <Card className="mb-6">
            <CardHeader>
              <CardTitle>{editingWorker ? 'Editar Trabalhador' : 'Criar Trabalhador'}</CardTitle>
            </CardHeader>
            <CardContent>
              <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-foreground mb-2">Nome</label>
                  <Input
                    type="text"
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-foreground mb-2">Cargo</label>
                  <Select
                    value={formData.role}
                    onChange={(e) => setFormData({ ...formData, role: e.target.value })}
                  >
                    <option value="nadador_salvador">Nadador-Salvador</option>
                    <option value="bar">Bar</option>
                    <option value="vigilante">Vigilante</option>
                    <option value="bilheteira">Bilheteira</option>
                  </Select>
                </div>
                <div className="flex gap-4">
                  <Button type="submit">{editingWorker ? 'Atualizar' : 'Criar'}</Button>
                  <Button type="button" variant="outline" onClick={handleCancelEdit}>
                    Cancelar
                  </Button>
                </div>
              </form>
            </CardContent>
          </Card>
        )}

        <Card>
          <CardHeader>
            <CardTitle>Trabalhadores</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead className="bg-muted">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-muted-foreground uppercase">
                      ID
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-muted-foreground uppercase">
                      Nome
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-muted-foreground uppercase">
                      Cargo
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-muted-foreground uppercase">
                      Estado
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-muted-foreground uppercase">
                      Ações
                    </th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-border">
                  {workers.map((worker) => (
                    <tr key={worker.id}>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-foreground">
                        {worker.workerId}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-foreground">
                        {worker.name}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-foreground">
                        {worker.role}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <Badge variant={worker.onShift ? "default" : "secondary"}>
                          {worker.onShift ? 'Em turno' : 'Fora de turno'}
                        </Badge>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm">
                        <div className="flex gap-2">
                          {worker.onShift ? (
                            <Button
                              variant="ghost"
                              size="icon"
                              onClick={() => handleDeactivateShift(worker.workerId)}
                              title="Desativar turno"
                            >
                              <X className="w-5 h-5" />
                            </Button>
                          ) : (
                            <>
                              <Button
                                variant="ghost"
                                size="icon"
                                onClick={() => handleActivateShift(worker.workerId, 'Manha')}
                                title="Ativar turno manhã"
                              >
                                <Clock className="w-5 h-5" />
                              </Button>
                              <Button
                                variant="ghost"
                                size="icon"
                                onClick={() => handleActivateShift(worker.workerId, 'Tarde')}
                                title="Ativar turno tarde"
                                className="text-orange-500"
                              >
                                <Clock className="w-5 h-5" />
                              </Button>
                            </>
                          )}
                          <Button
                            variant="ghost"
                            size="icon"
                            onClick={() => handleEdit(worker)}
                            title="Editar trabalhador"
                          >
                            <Edit className="w-5 h-5" />
                          </Button>
                          <Button
                            variant="ghost"
                            size="icon"
                            onClick={() => handleDelete(worker.workerId)}
                            title="Eliminar trabalhador"
                            className="text-destructive"
                          >
                            <Trash2 className="w-5 h-5" />
                          </Button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
};

export default WorkersPage;
