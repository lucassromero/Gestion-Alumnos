import { Navigate, Route, Routes } from 'react-router-dom'
import { Layout } from './components/Layout'
import { AlumnosPage } from './pages/AlumnosPage'
import { DetalleAlumnoPage } from './pages/DetalleAlumnoPage'
import { NuevoAlumnoPage } from './pages/NuevoAlumnoPage'
import { DetalleMateriaPage } from './pages/DetalleMateriaPage'
import { MateriasPage } from './pages/MateriasPage'
import { NuevaMateriaPage } from './pages/NuevaMateriaPage'
import { NotFoundPage } from './pages/NotFoundPage'

function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route index element={<Navigate to="/alumnos" replace />} />
        <Route path="alumnos" element={<AlumnosPage />} />
        <Route path="alumnos/nuevo" element={<NuevoAlumnoPage />} />
        <Route path="alumnos/:id" element={<DetalleAlumnoPage />} />
        <Route path="materias" element={<MateriasPage />} />
        <Route path="materias/nueva" element={<NuevaMateriaPage />} />
        <Route path="materias/:id" element={<DetalleMateriaPage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  )
}

export default App
