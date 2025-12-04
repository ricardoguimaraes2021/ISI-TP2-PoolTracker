import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  LayoutDashboard,
  Users,
  Droplets,
  Sparkles,
  FileText,
  LogOut,
  Plus,
  Minus,
  Power,
  Settings,
  Thermometer,
  TrendingUp,
  Calendar,
  Clock,
  CheckCircle2,
  XCircle,
  AlertCircle,
  RefreshCw,
  ShoppingCart,
  Edit,
  Trash2,
  X
} from 'lucide-react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, BarChart, Bar } from 'recharts';
import api from '../services/api';
import { logout } from '../services/auth';
import toast from 'react-hot-toast';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../components/ui/card';
import { Button } from '../components/ui/button';
import { Badge } from '../components/ui/badge';
import { Switch } from '../components/ui/switch';
import { Input } from '../components/ui/input';
import { Select } from '../components/ui/select';

const AdminDashboard = ({ initialTab = 'dashboard' }) => {
  const [poolStatus, setPoolStatus] = useState(null);
  const [weather, setWeather] = useState(null);
  const [activeTab, setActiveTab] = useState(initialTab);
  const [customCapacity, setCustomCapacity] = useState('');

  // Workers
  const [workers, setWorkers] = useState([]);
  const [activeWorkers, setActiveWorkers] = useState([]);
  const [newWorker, setNewWorker] = useState({ name: '', role: 'nadador_salvador' });
  const [loadingWorkers, setLoadingWorkers] = useState(false);
  const [workerRoleFilter, setWorkerRoleFilter] = useState('');
  const [editingWorker, setEditingWorker] = useState(null);

  // Water Quality
  const [waterQuality, setWaterQuality] = useState({ criancas: null, adultos: null });
  const [waterHistory, setWaterHistory] = useState([]);
  const [newMeasurement, setNewMeasurement] = useState({ poolType: 'Criancas', phLevel: '', temperature: '', notes: '' });
  const [historyDateRange, setHistoryDateRange] = useState({
    start: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
    end: new Date().toISOString().split('T')[0]
  });

  // Cleanings
  const [cleanings, setCleanings] = useState(null);
  const [newCleaning, setNewCleaning] = useState({ cleaningType: 'Balnearios', notes: '' });

  // Reports & Visits
  const [visits, setVisits] = useState([]);
  const [reports, setReports] = useState([]);
  const [shiftStats, setShiftStats] = useState([]);
  const [shiftDateRange, setShiftDateRange] = useState({
    start: new Date(new Date().getFullYear(), new Date().getMonth(), 1).toISOString().split('T')[0],
    end: new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).toISOString().split('T')[0]
  });

  // Shopping List
  const [shoppingItems, setShoppingItems] = useState([]);
  const [newShoppingItem, setNewShoppingItem] = useState({ name: '', category: 'Bar' });
  const [editingItem, setEditingItem] = useState(null);
  const [shoppingFilter, setShoppingFilter] = useState('');

  const navigate = useNavigate();

  // Fetch functions
  const fetchData = async () => {
    try {
      const [poolRes, weatherRes] = await Promise.allSettled([
        api.get('/api/pool/status'),
        api.get('/api/weather/current'),
      ]);
      if (poolRes.status === 'fulfilled') setPoolStatus(poolRes.value.data);
      if (weatherRes.status === 'fulfilled') setWeather(weatherRes.value.data);
    } catch (err) {
      toast.error('Erro ao carregar dados');
    }
  };

  const fetchWorkers = async () => {
    try {
      setLoadingWorkers(true);
      const res = await api.get('/api/workers');
      setWorkers(res.data);
    } catch (err) {
      toast.error('Erro ao carregar trabalhadores');
    } finally {
      setLoadingWorkers(false);
    }
  };

  const fetchActiveWorkers = async () => {
    try {
      const res = await api.get('/api/workers/active');
      setActiveWorkers(res.data.workers || []);
    } catch (err) {
      // Silencioso
    }
  };

  const fetchWaterQuality = async () => {
    try {
      const [criancasRes, adultosRes] = await Promise.allSettled([
        api.get('/api/water-quality/latest?poolType=Criancas'),
        api.get('/api/water-quality/latest?poolType=Adultos'),
      ]);
      setWaterQuality({
        criancas: criancasRes.status === 'fulfilled' ? criancasRes.value.data : null,
        adultos: adultosRes.status === 'fulfilled' ? adultosRes.value.data : null,
      });
    } catch (err) {
      console.error(err);
    }
  };

  const fetchWaterHistory = async () => {
    try {
      const startDate = historyDateRange.start || new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0];
      const endDate = historyDateRange.end || new Date().toISOString().split('T')[0];
      const res = await api.get(`/api/water-quality?startDate=${startDate}&endDate=${endDate}`);
      setWaterHistory(res.data);
    } catch (err) {
      console.error(err);
    }
  };

  const fetchCleanings = async () => {
    try {
      const res = await api.get('/api/cleaning/latest');
      setCleanings(res.data);
    } catch (err) {
      console.error(err);
    }
  };

  const fetchVisits = async () => {
    try {
      const startDate = new Date();
      startDate.setDate(startDate.getDate() - 30);
      const res = await api.get(`/api/statistics/visitors?startDate=${startDate.toISOString().split('T')[0]}&endDate=${new Date().toISOString().split('T')[0]}`);
      if (res.data && res.data.data) {
        setVisits(res.data.data);
      }
    } catch (err) {
      console.error(err);
    }
  };

  const fetchReports = async () => {
    try {
      const startDate = new Date();
      startDate.setDate(startDate.getDate() - 30);
      const res = await api.get(`/api/reports?startDate=${startDate.toISOString().split('T')[0]}&endDate=${new Date().toISOString().split('T')[0]}`);
      setReports(Array.isArray(res.data) ? res.data : []);
    } catch (err) {
      console.error(err);
    }
  };

  const fetchShiftStats = async () => {
    try {
      const res = await api.get(`/api/workers/shift-stats?startDate=${shiftDateRange.start}&endDate=${shiftDateRange.end}`);
      setShiftStats(res.data);
    } catch (err) {
      console.error(err);
    }
  };

  const fetchShoppingItems = async () => {
    try {
      const url = shoppingFilter ? `/api/shopping?category=${shoppingFilter}` : '/api/shopping';
      const res = await api.get(url);
      setShoppingItems(res.data);
    } catch (err) {
      toast.error('Erro ao carregar lista de compras');
    }
  };

  useEffect(() => {
    setActiveTab(initialTab);
  }, [initialTab]);

  useEffect(() => {
    fetchData();
    fetchWorkers();
    fetchActiveWorkers();
    fetchWaterQuality();
    fetchCleanings();
    fetchVisits();
    fetchReports();

    const interval = setInterval(() => {
      fetchData();
      fetchActiveWorkers();
      if (activeTab === 'workers') fetchWorkers();
      if (activeTab === 'water') {
        fetchWaterQuality();
        fetchWaterHistory();
      }
      if (activeTab === 'cleanings') fetchCleanings();
      if (activeTab === 'reports') {
        fetchVisits();
        fetchReports();
        fetchShiftStats();
      }
      if (activeTab === 'shopping') {
        fetchShoppingItems();
      }
    }, 10000);
    return () => clearInterval(interval);
  }, [activeTab]);

  useEffect(() => {
    if (activeTab === 'water') {
      fetchWaterHistory();
    }
    if (activeTab === 'reports') {
      fetchShiftStats();
    }
    if (activeTab === 'shopping') {
      fetchShoppingItems();
    }
  }, [historyDateRange, shiftDateRange, shoppingFilter, activeTab]);

  // Pool actions
  const enter = async () => {
    try {
      await api.post('/api/pool/enter');
      toast.success('Visitante registado');
      fetchData();
    } catch (err) {
      toast.error('Erro ao registar entrada');
    }
  };

  const exit = async () => {
    try {
      await api.post('/api/pool/exit');
      toast.success('Saída registada');
      fetchData();
    } catch (err) {
      toast.error('Erro ao registar saída');
    }
  };

  const toggleOpen = async (newState) => {
    if (poolStatus?.isOpen && !newState) {
      const confirmed = window.confirm(
        "⚠️ ATENÇÃO: Ao fechar a piscina, todos os turnos ativos dos funcionários serão terminados automaticamente.\n\n" +
        "Tem certeza que deseja fechar a piscina?"
      );
      if (!confirmed) return;
    }

    try {
      await api.put(`/api/pool/open-status?isOpen=${newState}`);
      toast.success(newState ? 'Piscina aberta' : 'Piscina encerrada - Todos os turnos foram finalizados');
      fetchData();
      fetchActiveWorkers();
    } catch (err) {
      toast.error('Erro ao alterar estado');
    }
  };

  const updateCapacity = async () => {
    const value = Number(customCapacity);
    if (!value || value <= 0) {
      toast.error('A capacidade tem de ser um número positivo.');
      return;
    }
    try {
      await api.put(`/api/pool/capacity?value=${value}`);
      toast.success('Capacidade atualizada');
      setCustomCapacity('');
      fetchData();
    } catch (err) {
      toast.error('Erro ao atualizar capacidade');
    }
  };

  // Worker actions
  const createWorker = async () => {
    if (!newWorker.name) {
      toast.error('Preencha o nome do trabalhador.');
      return;
    }
    try {
      await api.post('/api/workers', newWorker);
      toast.success('Trabalhador criado com sucesso');
      setNewWorker({ name: '', role: 'nadador_salvador' });
      fetchWorkers();
    } catch (err) {
      toast.error(err.response?.data?.error || 'Erro ao criar trabalhador.');
    }
  };

  const activateWorker = async (workerId, shiftType = 'Manha') => {
    try {
      await api.post(`/api/workers/${workerId}/activate`, { shiftType });
      toast.success('Turno iniciado com sucesso');
      fetchWorkers();
      fetchActiveWorkers();
    } catch (err) {
      toast.error('Erro ao iniciar turno');
    }
  };

  const deactivateWorker = async (workerId) => {
    try {
      await api.post(`/api/workers/${workerId}/deactivate`);
      toast.success('Turno finalizado com sucesso');
      fetchWorkers();
      fetchActiveWorkers();
    } catch (err) {
      toast.error('Erro ao finalizar turno');
    }
  };

  const updateWorker = async (workerId, name, role) => {
    try {
      await api.put(`/api/workers/${workerId}`, { name, role });
      toast.success('Trabalhador atualizado com sucesso');
      setEditingWorker(null);
      fetchWorkers();
    } catch (err) {
      toast.error(err.response?.data?.error || 'Erro ao atualizar trabalhador');
    }
  };

  const deleteWorker = async (workerId) => {
    if (!window.confirm('Tem certeza que deseja eliminar este trabalhador?')) {
      return;
    }
    try {
      await api.delete(`/api/workers/${workerId}`);
      toast.success('Trabalhador eliminado com sucesso');
      fetchWorkers();
    } catch (err) {
      toast.error(err.response?.data?.error || 'Erro ao eliminar trabalhador');
    }
  };

  // Water Quality actions
  const recordMeasurement = async () => {
    if (!newMeasurement.phLevel || !newMeasurement.temperature) {
      toast.error('Preencha pH e Temperatura.');
      return;
    }
    try {
      await api.post('/api/water-quality', {
        poolType: newMeasurement.poolType,
        phLevel: parseFloat(newMeasurement.phLevel),
        temperature: parseFloat(newMeasurement.temperature),
        notes: newMeasurement.notes || null,
      });
      toast.success('Medição registada com sucesso');
      setNewMeasurement({ poolType: 'Criancas', phLevel: '', temperature: '', notes: '' });
      fetchWaterQuality();
      fetchWaterHistory();
    } catch (err) {
      toast.error('Erro ao registrar medição.');
    }
  };

  // Cleaning actions
  const recordCleaning = async () => {
    try {
      await api.post('/api/cleaning', {
        cleaningType: newCleaning.cleaningType,
        notes: newCleaning.notes || null,
      });
      toast.success('Limpeza registada com sucesso');
      setNewCleaning({ cleaningType: 'Balnearios', notes: '' });
      fetchCleanings();
    } catch (err) {
      toast.error('Erro ao registrar limpeza.');
    }
  };

  // Shopping actions
  const createShoppingItem = async () => {
    if (!newShoppingItem.name) {
      toast.error('Preencha o nome do item.');
      return;
    }
    try {
      await api.post('/api/shopping', newShoppingItem);
      toast.success('Item adicionado com sucesso');
      setNewShoppingItem({ name: '', category: 'Bar' });
      fetchShoppingItems();
    } catch (err) {
      toast.error('Erro ao adicionar item');
    }
  };

  const updateShoppingItem = async (id, name, category) => {
    try {
      await api.put(`/api/shopping/${id}`, { name, category });
      toast.success('Item atualizado com sucesso');
      setEditingItem(null);
      fetchShoppingItems();
    } catch (err) {
      toast.error('Erro ao atualizar item');
    }
  };

  const deleteShoppingItem = async (id) => {
    if (!window.confirm('Tem certeza que deseja eliminar este item?')) {
      return;
    }
    try {
      await api.delete(`/api/shopping/${id}`);
      toast.success('Item eliminado com sucesso');
      fetchShoppingItems();
    } catch (err) {
      toast.error('Erro ao eliminar item');
    }
  };

  const handleLogout = () => {
    logout();
    navigate('/admin/login');
    toast.success('Sessão terminada');
  };

  if (!poolStatus) {
    return (
      <div className="min-h-screen bg-background flex items-center justify-center">
        <div className="flex items-center gap-2 text-muted-foreground">
          <RefreshCw className="w-4 h-4 animate-spin" />
          <span>A carregar...</span>
        </div>
      </div>
    );
  }

  const roleLabels = {
    nadador_salvador: 'Nadador Salvador',
    vigilante: 'Vigilante',
    bar: 'Bar',
    bilheteira: 'Bilheteira',
  };

  const occupancyPercentage = poolStatus
    ? Math.round((poolStatus.currentCount / poolStatus.maxCapacity) * 100)
    : 0;

  // Prepare chart data
  const visitsChartData = visits.map(v => ({
    date: new Date(v.date || v.Date).toLocaleDateString('pt-PT', { day: '2-digit', month: '2-digit' }),
    visitantes: v.visitors || v.Visitors || 0
  }));

  const waterChartData = waterHistory.reduce((acc, m) => {
    const date = new Date(m.measuredAt).toLocaleDateString('pt-PT', { day: '2-digit', month: '2-digit' });
    const existing = acc.find(item => item.date === date);
    if (existing) {
      if (m.poolType === 'Criancas') {
        existing.phCriancas = m.phLevel;
        existing.tempCriancas = m.temperature;
      } else {
        existing.phAdultos = m.phLevel;
        existing.tempAdultos = m.temperature;
      }
    } else {
      acc.push({
        date,
        phCriancas: m.poolType === 'Criancas' ? m.phLevel : null,
        tempCriancas: m.poolType === 'Criancas' ? m.temperature : null,
        phAdultos: m.poolType === 'Adultos' ? m.phLevel : null,
        tempAdultos: m.poolType === 'Adultos' ? m.temperature : null,
      });
    }
    return acc;
  }, []).slice(-30);

  const tabs = [
    { id: 'dashboard', label: 'Dashboard', icon: LayoutDashboard },
    { id: 'workers', label: 'Trabalhadores', icon: Users },
    { id: 'water', label: 'Qualidade Água', icon: Droplets },
    { id: 'cleanings', label: 'Limpezas', icon: Sparkles },
    { id: 'reports', label: 'Relatórios', icon: FileText },
    { id: 'shopping', label: 'Compras', icon: ShoppingCart },
  ];

  return (
    <div className="min-h-screen bg-background text-foreground">
      <div className="border-b border-border">
        <div className="container mx-auto px-2 sm:px-4 py-3 sm:py-4 flex flex-col sm:flex-row items-start sm:items-center justify-between gap-2">
          <h1 className="text-xl sm:text-2xl font-bold">Painel Administrativo</h1>
          <Button
            variant="ghost"
            size="sm"
            onClick={handleLogout}
          >
            <LogOut className="w-4 h-4 sm:mr-2" />
            <span className="hidden sm:inline">Logout</span>
          </Button>
        </div>

        {/* Tabs */}
        <div className="container mx-auto px-2 sm:px-4 flex gap-1 border-t border-border overflow-x-auto">
          {tabs.map((tab) => {
            const Icon = tab.icon;
            return (
              <button
                key={tab.id}
                onClick={() => setActiveTab(tab.id)}
                className={`px-2 sm:px-4 py-2 sm:py-3 text-xs sm:text-sm font-medium flex items-center gap-1 sm:gap-2 border-b-2 transition-colors whitespace-nowrap ${
                  activeTab === tab.id
                    ? 'border-primary text-primary'
                    : 'border-transparent text-muted-foreground hover:text-foreground'
                }`}
              >
                <Icon className="w-4 h-4" />
                {tab.label}
              </button>
            );
          })}
        </div>
      </div>

      <div className="container mx-auto px-2 sm:px-4 py-4 sm:py-8 space-y-6">
        {/* Dashboard Tab */}
        {activeTab === 'dashboard' && (
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <Card>
              <CardHeader>
                <CardTitle className="text-sm font-medium flex items-center gap-2">
                  <Power className="w-4 h-4" />
                  Estado
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="flex items-center justify-between gap-4">
                  <div className="flex flex-col gap-1">
                    <Badge variant={poolStatus.isOpen ? 'default' : 'destructive'} className="text-base w-fit">
                      {poolStatus.isOpen ? 'Aberta' : 'Encerrada'}
                    </Badge>
                    {poolStatus.isOpen && (
                      <p className="text-xs text-muted-foreground">
                        Ao fechar, todos os turnos serão finalizados
                      </p>
                    )}
                  </div>
                  <div className="flex items-center gap-3">
                    <span className="text-sm text-muted-foreground hidden sm:inline">
                      {poolStatus.isOpen ? 'Fechar' : 'Abrir'}
                    </span>
                    <Switch
                      checked={poolStatus.isOpen}
                      onCheckedChange={toggleOpen}
                    />
                  </div>
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle className="text-sm font-medium">Lotação Atual</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="text-2xl sm:text-3xl font-bold mb-4">
                  {poolStatus.currentCount} / {poolStatus.maxCapacity}
                </div>
                <div className="flex gap-2">
                  <Button
                    onClick={enter}
                    disabled={!poolStatus.isOpen || poolStatus.currentCount >= poolStatus.maxCapacity}
                    size="sm"
                    variant="default"
                    className="flex-1"
                  >
                    <Plus className="w-4 h-4 mr-1" />
                    Entrou
                  </Button>
                  <Button
                    onClick={exit}
                    disabled={poolStatus.currentCount === 0}
                    size="sm"
                    variant="destructive"
                    className="flex-1"
                  >
                    <Minus className="w-4 h-4 mr-1" />
                    Saiu
                  </Button>
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle className="text-sm font-medium flex items-center gap-2">
                  <Thermometer className="w-4 h-4" />
                  Meteorologia
                </CardTitle>
              </CardHeader>
              <CardContent>
                {weather ? (
                  <>
                    <div className="text-2xl sm:text-3xl font-bold">{Math.round(weather.temperatureC || weather.temperature || 0)}°C</div>
                    <p className="text-sm text-muted-foreground mt-1">{weather.description || 'N/A'}</p>
                    <p className="text-xs text-muted-foreground mt-1">Vento: {(weather.windSpeedKmh || weather.windSpeed || 0).toFixed(1)} km/h</p>
                  </>
                ) : (
                  <p className="text-sm text-muted-foreground">Sem dados</p>
                )}
              </CardContent>
            </Card>

            <Card className="md:col-span-3">
              <CardHeader>
                <CardTitle className="text-sm font-medium flex items-center gap-2">
                  <Users className="w-4 h-4" />
                  Trabalhadores Ativos no Turno
                </CardTitle>
              </CardHeader>
              <CardContent>
                {activeWorkers.length > 0 ? (
                  <div className="space-y-2">
                    {activeWorkers.map((worker) => (
                      <div key={worker.workerId} className="flex items-center justify-between p-2 rounded-lg bg-muted/50">
                        <div className="flex-1">
                          <div className="font-medium">{worker.name}</div>
                          <div className="text-sm text-muted-foreground">
                            {roleLabels[worker.role] || worker.role}
                          </div>
                        </div>
                        <div className="text-xs text-muted-foreground">
                          {worker.startTime ? new Date(worker.startTime).toLocaleTimeString('pt-PT', { hour: '2-digit', minute: '2-digit' }) : ''}
                        </div>
                      </div>
                    ))}
                  </div>
                ) : (
                  <p className="text-sm text-muted-foreground">Nenhum trabalhador ativo no momento</p>
                )}
              </CardContent>
            </Card>

            <Card className="md:col-span-3">
              <CardHeader>
                <CardTitle className="text-sm font-medium flex items-center gap-2">
                  <Settings className="w-4 h-4" />
                  Alterar Capacidade
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="flex gap-2">
                  <Input
                    value={customCapacity}
                    onChange={(e) => setCustomCapacity(e.target.value)}
                    type="number"
                    placeholder="Nova capacidade..."
                    className="max-w-xs"
                  />
                  <Button onClick={updateCapacity}>Atualizar</Button>
                </div>
              </CardContent>
            </Card>
          </div>
        )}

        {/* Workers Tab */}
        {activeTab === 'workers' && (
          <div className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle>Adicionar Trabalhador</CardTitle>
                <CardDescription>O ID será gerado automaticamente</CardDescription>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
                  <Input
                    value={newWorker.name}
                    onChange={(e) => setNewWorker({ ...newWorker, name: e.target.value })}
                    placeholder="Nome"
                  />
                  <Select
                    value={newWorker.role}
                    onChange={(e) => setNewWorker({ ...newWorker, role: e.target.value })}
                  >
                    <option value="nadador_salvador">Nadador Salvador</option>
                    <option value="vigilante">Vigilante</option>
                    <option value="bar">Bar</option>
                    <option value="bilheteira">Bilheteira</option>
                  </Select>
                  <Button onClick={createWorker} disabled={loadingWorkers}>
                    <Plus className="w-4 h-4 mr-2" />
                    Adicionar
                  </Button>
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Lista de Trabalhadores</CardTitle>
                <CardDescription>
                  {workerRoleFilter
                    ? `Filtrado por: ${roleLabels[workerRoleFilter] || workerRoleFilter} (${workers.filter(w => !workerRoleFilter || w.role === workerRoleFilter).length} trabalhadores)`
                    : `Total: ${workers.length} trabalhadores`
                  }
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="mb-4 flex flex-col sm:flex-row gap-3">
                  <Select
                    value={workerRoleFilter}
                    onChange={(e) => setWorkerRoleFilter(e.target.value)}
                    className="max-w-xs"
                  >
                    <option value="">Todos os cargos</option>
                    <option value="nadador_salvador">Nadador Salvador</option>
                    <option value="vigilante">Vigilante</option>
                    <option value="bar">Bar</option>
                    <option value="bilheteira">Bilheteira</option>
                  </Select>
                  <Button onClick={fetchWorkers} variant="outline" size="sm">
                    <RefreshCw className="w-4 h-4 mr-2" />
                    Atualizar
                  </Button>
                </div>
                <div className="overflow-x-auto">
                  <table className="w-full text-sm">
                    <thead>
                      <tr className="border-b border-border">
                        <th className="text-left p-2">ID</th>
                        <th className="text-left p-2">Nome</th>
                        <th className="text-left p-2">Cargo</th>
                        <th className="text-left p-2">Estado</th>
                        <th className="text-left p-2">Turno</th>
                        <th className="text-left p-2">Ações</th>
                      </tr>
                    </thead>
                    <tbody>
                      {workers
                        .filter(worker => !workerRoleFilter || worker.role === workerRoleFilter)
                        .map((worker) => (
                          <tr key={worker.id} className="border-b border-border/50">
                            <td className="p-2 font-mono text-xs">{worker.workerId}</td>
                            <td className="p-2">{worker.name}</td>
                            <td className="p-2">{roleLabels[worker.role] || worker.role}</td>
                            <td className="p-2">
                              <Badge variant={worker.isActive ? 'default' : 'secondary'}>
                                {worker.isActive ? 'Ativo' : 'Inativo'}
                              </Badge>
                            </td>
                            <td className="p-2">
                              {worker.onShift ? (
                                <Badge variant="default" className="bg-emerald-600">
                                  <CheckCircle2 className="w-3 h-3 mr-1" />
                                  Em Turno
                                </Badge>
                              ) : (
                                <Badge variant="outline">Sem Turno</Badge>
                              )}
                            </td>
                            <td className="p-2">
                              <div className="flex flex-wrap gap-2">
                                {!worker.onShift && (
                                  <>
                                    <Button
                                      onClick={() => activateWorker(worker.workerId, 'Manha')}
                                      disabled={!poolStatus?.isOpen}
                                      size="sm"
                                      variant="default"
                                      className="text-xs"
                                    >
                                      <Clock className="w-3 h-3 mr-1" />
                                      <span className="hidden sm:inline">Manhã</span>
                                    </Button>
                                    <Button
                                      onClick={() => activateWorker(worker.workerId, 'Tarde')}
                                      disabled={!poolStatus?.isOpen}
                                      size="sm"
                                      variant="outline"
                                      className="text-xs"
                                    >
                                      <Clock className="w-3 h-3 mr-1" />
                                      <span className="hidden sm:inline">Tarde</span>
                                    </Button>
                                  </>
                                )}
                                {worker.onShift && (
                                  <Button
                                    onClick={() => deactivateWorker(worker.workerId)}
                                    size="sm"
                                    variant="outline"
                                    className="text-xs"
                                  >
                                    <XCircle className="w-3 h-3 mr-1" />
                                    <span className="hidden sm:inline">Finalizar</span>
                                  </Button>
                                )}
                                <Button
                                  onClick={() => setEditingWorker({ ...worker })}
                                  size="sm"
                                  variant="ghost"
                                  className="text-xs"
                                >
                                  <Edit className="w-3 h-3 mr-1" />
                                  <span className="hidden sm:inline">Editar</span>
                                </Button>
                                <Button
                                  onClick={() => deleteWorker(worker.workerId)}
                                  size="sm"
                                  variant="ghost"
                                  className="text-xs text-destructive hover:text-destructive"
                                >
                                  <Trash2 className="w-3 h-3 mr-1" />
                                  <span className="hidden sm:inline">Eliminar</span>
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

            {/* Modal de Edição de Trabalhador */}
            {editingWorker && (
              <Card className="border-primary">
                <CardHeader>
                  <CardTitle>Editar Trabalhador</CardTitle>
                  <CardDescription>ID: {editingWorker.workerId} (não editável)</CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="space-y-3">
                    <div>
                      <label className="text-sm font-medium mb-1 block">Nome</label>
                      <Input
                        value={editingWorker.name}
                        onChange={(e) => setEditingWorker({ ...editingWorker, name: e.target.value })}
                        placeholder="Nome do trabalhador"
                      />
                    </div>
                    <div>
                      <label className="text-sm font-medium mb-1 block">Cargo</label>
                      <Select
                        value={editingWorker.role}
                        onChange={(e) => setEditingWorker({ ...editingWorker, role: e.target.value })}
                      >
                        <option value="nadador_salvador">Nadador Salvador</option>
                        <option value="vigilante">Vigilante</option>
                        <option value="bar">Bar</option>
                        <option value="bilheteira">Bilheteira</option>
                      </Select>
                    </div>
                    <div className="flex gap-2">
                      <Button
                        onClick={() => updateWorker(editingWorker.workerId, editingWorker.name, editingWorker.role)}
                      >
                        Guardar
                      </Button>
                      <Button
                        variant="outline"
                        onClick={() => setEditingWorker(null)}
                      >
                        Cancelar
                      </Button>
                    </div>
                  </div>
                </CardContent>
              </Card>
            )}
          </div>
        )}

        {/* Water Quality Tab */}
        {activeTab === 'water' && (
          <div className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle>Registar Medição</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-1 md:grid-cols-4 gap-3">
                  <Select
                    value={newMeasurement.poolType}
                    onChange={(e) => setNewMeasurement({ ...newMeasurement, poolType: e.target.value })}
                  >
                    <option value="Criancas">Piscina Crianças</option>
                    <option value="Adultos">Piscina Adultos</option>
                  </Select>
                  <Input
                    type="number"
                    step="0.1"
                    value={newMeasurement.phLevel}
                    onChange={(e) => setNewMeasurement({ ...newMeasurement, phLevel: e.target.value })}
                    placeholder="pH (ex: 7.2)"
                  />
                  <Input
                    type="number"
                    step="0.1"
                    value={newMeasurement.temperature}
                    onChange={(e) => setNewMeasurement({ ...newMeasurement, temperature: e.target.value })}
                    placeholder="Temperatura °C"
                  />
                  <Button onClick={recordMeasurement}>
                    <Droplets className="w-4 h-4 mr-2" />
                    Registrar
                  </Button>
                </div>
              </CardContent>
            </Card>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <Card>
                <CardHeader>
                  <CardTitle>Piscina Crianças</CardTitle>
                </CardHeader>
                <CardContent>
                  {waterQuality.criancas ? (
                    <div className="space-y-3">
                      <div className="flex items-center gap-4">
                        <div>
                          <p className="text-xs text-muted-foreground">pH</p>
                          <p className="text-2xl font-bold">{waterQuality.criancas.phLevel?.toFixed(1) || waterQuality.criancas.PhLevel?.toFixed(1) || 'N/A'}</p>
                        </div>
                        <div>
                          <p className="text-xs text-muted-foreground">Temperatura</p>
                          <p className="text-2xl font-bold">{waterQuality.criancas.temperature?.toFixed(1) || waterQuality.criancas.Temperature?.toFixed(1) || 'N/A'}°C</p>
                        </div>
                      </div>
                      {waterQuality.criancas.measuredAt && (
                        <p className="text-xs text-muted-foreground">
                          <Clock className="w-3 h-3 inline mr-1" />
                          {new Date(waterQuality.criancas.measuredAt).toLocaleString('pt-PT')}
                        </p>
                      )}
                    </div>
                  ) : (
                    <p className="text-muted-foreground">Sem medições</p>
                  )}
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle>Piscina Adultos</CardTitle>
                </CardHeader>
                <CardContent>
                  {waterQuality.adultos ? (
                    <div className="space-y-3">
                      <div className="flex items-center gap-4">
                        <div>
                          <p className="text-xs text-muted-foreground">pH</p>
                          <p className="text-2xl font-bold">{waterQuality.adultos.phLevel?.toFixed(1) || waterQuality.adultos.PhLevel?.toFixed(1) || 'N/A'}</p>
                        </div>
                        <div>
                          <p className="text-xs text-muted-foreground">Temperatura</p>
                          <p className="text-2xl font-bold">{waterQuality.adultos.temperature?.toFixed(1) || waterQuality.adultos.Temperature?.toFixed(1) || 'N/A'}°C</p>
                        </div>
                      </div>
                      {waterQuality.adultos.measuredAt && (
                        <p className="text-xs text-muted-foreground">
                          <Clock className="w-3 h-3 inline mr-1" />
                          {new Date(waterQuality.adultos.measuredAt).toLocaleString('pt-PT')}
                        </p>
                      )}
                    </div>
                  ) : (
                    <p className="text-muted-foreground">Sem medições</p>
                  )}
                </CardContent>
              </Card>
            </div>

            <Card>
              <CardHeader>
                <CardTitle>Histórico de Qualidade da Água</CardTitle>
                <CardDescription>Selecione o período para visualizar o histórico</CardDescription>
              </CardHeader>
              <CardContent>
                <div className="flex gap-2 mb-4">
                  <Input
                    type="date"
                    value={historyDateRange.start}
                    onChange={(e) => setHistoryDateRange({ ...historyDateRange, start: e.target.value })}
                    className="max-w-xs"
                  />
                  <Input
                    type="date"
                    value={historyDateRange.end}
                    onChange={(e) => setHistoryDateRange({ ...historyDateRange, end: e.target.value })}
                    className="max-w-xs"
                  />
                </div>
                {waterChartData.length > 0 ? (
                  <div className="space-y-6">
                    <div>
                      <h3 className="text-sm font-medium mb-4">pH por Data</h3>
                      <ResponsiveContainer width="100%" height={300}>
                        <LineChart data={waterChartData}>
                          <CartesianGrid strokeDasharray="3 3" />
                          <XAxis dataKey="date" />
                          <YAxis />
                          <Tooltip />
                          <Legend />
                          <Line type="monotone" dataKey="phCriancas" stroke="#3b82f6" name="pH Crianças" />
                          <Line type="monotone" dataKey="phAdultos" stroke="#ef4444" name="pH Adultos" />
                        </LineChart>
                      </ResponsiveContainer>
                    </div>
                    <div>
                      <h3 className="text-sm font-medium mb-4">Temperatura por Data</h3>
                      <ResponsiveContainer width="100%" height={300}>
                        <LineChart data={waterChartData}>
                          <CartesianGrid strokeDasharray="3 3" />
                          <XAxis dataKey="date" />
                          <YAxis />
                          <Tooltip />
                          <Legend />
                          <Line type="monotone" dataKey="tempCriancas" stroke="#3b82f6" name="Temp. Crianças" />
                          <Line type="monotone" dataKey="tempAdultos" stroke="#ef4444" name="Temp. Adultos" />
                        </LineChart>
                      </ResponsiveContainer>
                    </div>
                  </div>
                ) : (
                  <p className="text-muted-foreground">Sem dados para o período selecionado</p>
                )}
              </CardContent>
            </Card>
          </div>
        )}

        {/* Cleanings Tab */}
        {activeTab === 'cleanings' && (
          <div className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle>Registar Limpeza</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
                  <Select
                    value={newCleaning.cleaningType}
                    onChange={(e) => setNewCleaning({ ...newCleaning, cleaningType: e.target.value })}
                  >
                    <option value="Balnearios">Balneários</option>
                    <option value="Wc">WC</option>
                  </Select>
                  <Input
                    value={newCleaning.notes}
                    onChange={(e) => setNewCleaning({ ...newCleaning, notes: e.target.value })}
                    placeholder="Notas (opcional)"
                  />
                  <Button onClick={recordCleaning}>
                    <Sparkles className="w-4 h-4 mr-2" />
                    Registrar
                  </Button>
                </div>
              </CardContent>
            </Card>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <Card>
                <CardHeader>
                  <CardTitle>Última Limpeza - Balneários</CardTitle>
                </CardHeader>
                <CardContent>
                  {cleanings?.balnearios ? (
                    <div>
                      <p className="text-xl font-semibold">
                        {new Date(cleanings.balnearios.cleanedAt || cleanings.balnearios.CleanedAt).toLocaleString('pt-PT')}
                      </p>
                      {cleanings.balnearios.notes && (
                        <p className="text-sm text-muted-foreground mt-2">{cleanings.balnearios.notes}</p>
                      )}
                    </div>
                  ) : (
                    <p className="text-muted-foreground">Sem registos</p>
                  )}
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle>Última Limpeza - WC</CardTitle>
                </CardHeader>
                <CardContent>
                  {cleanings?.wc ? (
                    <div>
                      <p className="text-xl font-semibold">
                        {new Date(cleanings.wc.cleanedAt || cleanings.wc.CleanedAt).toLocaleString('pt-PT')}
                      </p>
                      {cleanings.wc.notes && (
                        <p className="text-sm text-muted-foreground mt-2">{cleanings.wc.notes}</p>
                      )}
                    </div>
                  ) : (
                    <p className="text-muted-foreground">Sem registos</p>
                  )}
                </CardContent>
              </Card>
            </div>
          </div>
        )}

        {/* Reports Tab */}
        {activeTab === 'reports' && (
          <div className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle>Estatísticas de Visitantes</CardTitle>
                <CardDescription>Últimos 30 dias</CardDescription>
              </CardHeader>
              <CardContent>
                {visitsChartData.length > 0 ? (
                  <ResponsiveContainer width="100%" height={300}>
                    <LineChart data={visitsChartData}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="date" />
                      <YAxis />
                      <Tooltip />
                      <Line type="monotone" dataKey="visitantes" stroke="#3b82f6" strokeWidth={2} />
                    </LineChart>
                  </ResponsiveContainer>
                ) : (
                  <p className="text-muted-foreground">Sem dados disponíveis</p>
                )}
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Estatísticas de Turnos</CardTitle>
                <CardDescription>Selecione o período para visualizar as estatísticas</CardDescription>
              </CardHeader>
              <CardContent>
                <div className="flex gap-2 mb-4">
                  <Input
                    type="date"
                    value={shiftDateRange.start}
                    onChange={(e) => setShiftDateRange({ ...shiftDateRange, start: e.target.value })}
                    className="max-w-xs"
                  />
                  <Input
                    type="date"
                    value={shiftDateRange.end}
                    onChange={(e) => setShiftDateRange({ ...shiftDateRange, end: e.target.value })}
                    className="max-w-xs"
                  />
                </div>
                {shiftStats.length > 0 ? (
                  <div className="overflow-x-auto">
                    <table className="w-full text-sm">
                      <thead>
                        <tr className="border-b border-border">
                          <th className="text-left p-2">Trabalhador</th>
                          <th className="text-left p-2">Cargo</th>
                          <th className="text-left p-2">Manhã</th>
                          <th className="text-left p-2">Tarde</th>
                          <th className="text-left p-2">Total</th>
                        </tr>
                      </thead>
                      <tbody>
                        {shiftStats.map((stat) => (
                          <tr key={stat.workerId} className="border-b border-border/50">
                            <td className="p-2">{stat.name}</td>
                            <td className="p-2">{roleLabels[stat.role] || stat.role}</td>
                            <td className="p-2">{stat.manha || 0}</td>
                            <td className="p-2">{stat.tarde || 0}</td>
                            <td className="p-2 font-semibold">{stat.total || 0}</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                ) : (
                  <p className="text-muted-foreground">Sem dados para o período selecionado</p>
                )}
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Relatórios Diários</CardTitle>
                <CardDescription>Últimos 30 dias</CardDescription>
              </CardHeader>
              <CardContent>
                {reports.length > 0 ? (
                  <div className="space-y-4">
                    {reports.map((report) => (
                      <div key={report.id} className="p-4 rounded-lg border bg-muted/50">
                        <div className="flex items-center justify-between mb-2">
                          <h3 className="font-semibold">
                            {report.reportDate ? new Date(report.reportDate).toLocaleDateString('pt-PT') : 'Data não disponível'}
                          </h3>
                        </div>
                        <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm">
                          <div>
                            <p className="text-muted-foreground">Total Visitantes</p>
                            <p className="font-semibold">{report.totalVisitors ?? 0}</p>
                          </div>
                          <div>
                            <p className="text-muted-foreground">Ocupação Máxima</p>
                            <p className="font-semibold">{report.maxOccupancy ?? 0}</p>
                          </div>
                          <div>
                            <p className="text-muted-foreground">Ocupação Média</p>
                            <p className="font-semibold">{report.avgOccupancy ? report.avgOccupancy.toFixed(1) : 'N/A'}</p>
                          </div>
                          <div>
                            <p className="text-muted-foreground">Hora de Fecho</p>
                            <p className="font-semibold">
                              {report.closingTime ? new Date(report.closingTime).toLocaleTimeString('pt-PT') : 'N/A'}
                            </p>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                ) : (
                  <p className="text-muted-foreground">Não há relatórios disponíveis</p>
                )}
              </CardContent>
            </Card>
          </div>
        )}

        {/* Shopping Tab */}
        {activeTab === 'shopping' && (
          <div className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle>Adicionar Item</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
                  <Input
                    value={newShoppingItem.name}
                    onChange={(e) => setNewShoppingItem({ ...newShoppingItem, name: e.target.value })}
                    placeholder="Nome do item"
                  />
                  <Select
                    value={newShoppingItem.category}
                    onChange={(e) => setNewShoppingItem({ ...newShoppingItem, category: e.target.value })}
                  >
                    <option value="Bar">Bar</option>
                    <option value="Limpeza">Limpeza</option>
                    <option value="Qualidade">Qualidade</option>
                  </Select>
                  <Button onClick={createShoppingItem}>
                    <Plus className="w-4 h-4 mr-2" />
                    Adicionar
                  </Button>
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Lista de Compras</CardTitle>
                <CardDescription>Filtre por categoria para ver itens específicos</CardDescription>
              </CardHeader>
              <CardContent>
                <div className="flex gap-2 mb-4">
                  <Select
                    value={shoppingFilter}
                    onChange={(e) => setShoppingFilter(e.target.value)}
                    className="max-w-xs"
                  >
                    <option value="">Todas as categorias</option>
                    <option value="Bar">Bar</option>
                    <option value="Limpeza">Limpeza</option>
                    <option value="Qualidade">Qualidade</option>
                  </Select>
                </div>
                {shoppingItems.length > 0 ? (
                  <div className="space-y-2">
                    {shoppingItems.map((item) => (
                      <div key={item.id} className="flex items-center justify-between p-3 rounded-lg border bg-muted/50">
                        <div className="flex-1">
                          <div className="font-medium">{item.name}</div>
                          <div className="text-sm text-muted-foreground">
                            <Badge variant="outline">{item.category}</Badge>
                          </div>
                        </div>
                        <div className="flex gap-2">
                          <Button
                            onClick={() => setEditingItem({ ...item })}
                            size="sm"
                            variant="ghost"
                          >
                            <Edit className="w-4 h-4" />
                          </Button>
                          <Button
                            onClick={() => deleteShoppingItem(item.id)}
                            size="sm"
                            variant="ghost"
                            className="text-destructive"
                          >
                            <Trash2 className="w-4 h-4" />
                          </Button>
                        </div>
                      </div>
                    ))}
                  </div>
                ) : (
                  <p className="text-muted-foreground">Não há itens na lista</p>
                )}
              </CardContent>
            </Card>

            {/* Modal de Edição de Item */}
            {editingItem && (
              <Card className="border-primary">
                <CardHeader>
                  <CardTitle>Editar Item</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="space-y-3">
                    <div>
                      <label className="text-sm font-medium mb-1 block">Nome</label>
                      <Input
                        value={editingItem.name}
                        onChange={(e) => setEditingItem({ ...editingItem, name: e.target.value })}
                        placeholder="Nome do item"
                      />
                    </div>
                    <div>
                      <label className="text-sm font-medium mb-1 block">Categoria</label>
                      <Select
                        value={editingItem.category}
                        onChange={(e) => setEditingItem({ ...editingItem, category: e.target.value })}
                      >
                        <option value="Bar">Bar</option>
                        <option value="Limpeza">Limpeza</option>
                        <option value="Qualidade">Qualidade</option>
                      </Select>
                    </div>
                    <div className="flex gap-2">
                      <Button
                        onClick={() => updateShoppingItem(editingItem.id, editingItem.name, editingItem.category)}
                      >
                        Guardar
                      </Button>
                      <Button
                        variant="outline"
                        onClick={() => setEditingItem(null)}
                      >
                        Cancelar
                      </Button>
                    </div>
                  </div>
                </CardContent>
              </Card>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default AdminDashboard;
