export function EmptyState({ mensaje }: { mensaje: string }) {
  return (
    <div className="rounded border border-dashed border-borde bg-hoja p-10 text-center text-tinta-suave">
      <p className="text-sm">{mensaje}</p>
    </div>
  )
}
