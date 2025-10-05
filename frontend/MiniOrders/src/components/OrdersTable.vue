<template>
  <div class="p-4">
    <h2 class="text-2xl font-bold mb-4">Gestión de Órdenes</h2>

    <!-- Botón para abrir modal -->
    <button class="btn btn-primary mb-3" @click="openModal()"> Nueva Orden</button>

    <!-- Tabla -->
    <table class="table table-bordered table-hover">
      <thead class="table-dark">
        <tr>
          <th>ID</th>
          <th>Cliente</th>
          <th>Fecha</th>
          <th>Cantidad</th>
          <th>Total</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="order in orders" :key="order.id">
          <td>{{ order.id }}</td>
          <td>{{ order.cliente }}</td>
          <td>{{ new Date(order.fecha).toLocaleDateString() }}</td>
          <td>{{ order.cantidad }}</td>
          <td>{{ order.total.toLocaleString('es-DO', { style: 'currency', currency: 'DOP' }) }}</td>
          <td>
            <button class="btn btn-sm btn-warning me-2" @click="openModal(order)">Editar</button>
            <button class="btn btn-sm btn-danger" @click="deleteOrder(order.id)">Eliminar</button>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- MODAL -->
    <div class="modal fade" id="orderModal" tabindex="-1" ref="modalElement">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ editing ? 'Editar Orden' : 'Nueva Orden' }}</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" @click="resetForm"></button>
          </div>
          <form @submit.prevent="saveOrder">
            <div class="modal-body">
              <div class="mb-3">
                <label class="form-label">Cliente</label>
                <input v-model="form.cliente" class="form-control" required />
              </div>
              <div class="mb-3">
                <label class="form-label">Fecha</label>
                <input type="date" v-model="form.fecha" class="form-control" required />
              </div>
              <div class="mb-3">
                <label class="form-label">Cantidad</label>
                <input type="number" v-model="form.cantidad" class="form-control" required />
              </div>
              <div class="mb-3">
                <label class="form-label">Total</label>
                <input type="number" v-model="form.total" class="form-control" required />
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" data-bs-dismiss="modal" @click="resetForm">Cancelar</button>
              <button type="submit" class="btn btn-primary">{{ editing ? 'Actualizar' : 'Guardar' }}</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick } from 'vue'
import axios from 'axios'
import * as bootstrap from 'bootstrap'

const API_URL = 'http://localhost:5176/api/orders'
const orders = ref([])
const editing = ref(false)
const form = ref({ id: null, cliente: '', fecha: '', cantidad: 0, total: 0 })
const modalElement = ref(null)
let modalInstance = null

onMounted(async () => {
  await fetchOrders()
  await nextTick()
  modalInstance = new bootstrap.Modal(modalElement.value)
})

const fetchOrders = async () => {
  try {
    const res = await axios.get(API_URL)
    orders.value = res.data
  } catch (err) {
    console.error('Error al obtener órdenes:', err)
  }
}

const openModal = (order = null) => {
  if (order) {
    form.value = { ...order, fecha: order.fecha.split('T')[0] }
    editing.value = true
  } else {
    resetForm()
  }
  modalInstance.show()
}

const saveOrder = async () => {
  try {
    if (editing.value) {
      await axios.put(`${API_URL}/${form.value.id}`, form.value)
    } else {
      await axios.post(API_URL, form.value)
    }
    await fetchOrders()
    modalInstance.hide()
    resetForm()
  } catch (err) {
    console.error('Error al guardar la orden:', err)
  }
}

const deleteOrder = async (id) => {
  if (!confirm('¿Estás seguro de eliminar esta orden?')) return
  try {
    await axios.delete(`${API_URL}/${id}`)
    await fetchOrders()
  } catch (err) {
    console.error('Error al eliminar la orden:', err)
  }
}

const resetForm = () => {
  form.value = { id: null, cliente: '', fecha: '', cantidad: 0, total: 0 }
  editing.value = false
}
</script>

<style scoped>
.table {
  border-collapse: collapse;
  width: 100%;
}
.table th,
.table td {
  border: 1px solid #6b6666;
  padding: 8px;
}
.table th {
  background-color: #b7b7b7;
}
</style>
